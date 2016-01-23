using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ImageMaker.WebBrowsing
{
    public class InstagramExplorer
    {
        private const string cUserName = @"george.39reg";
        private const string cClientToken = @"fd2a555e04d54f1db8423c0e133fad91";

        public async Task<ImageResponse> GetImagesFromUrl(string url)
        {
            ImageResponse imageResponse = null;
            List<Image> images = new List<Image>();
            try
            {
                using (var client = new WebClient())
                {
                    string data = await client.DownloadStringTaskAsync(new Uri(url));
                    JObject token = JObject.Parse(data);

                    JObject paginationToken = JObject.FromObject(token.SelectToken("pagination"));
                    imageResponse = paginationToken.ToObject<ImageResponse>();
                    foreach (var tokenData in token.SelectToken("data"))
                    {
                        JObject jObject = JObject.FromObject(new { id = tokenData.SelectToken("id").Value<string>() });
                        JToken imageToken = tokenData.SelectToken("images").SelectToken("standard_resolution");
                        JObject imageObject = JObject.FromObject(imageToken);
                        JToken userDataToken =JObject.FromObject(tokenData.SelectToken("user"));
                        string fullName = userDataToken.SelectToken("full_name").Value<string>();
                        JObject userObject=JObject.FromObject(new
                        {
                            fullname=string.IsNullOrEmpty(fullName) ? userDataToken.SelectToken("username").Value<string>() : fullName,
                        });
                        JObject profilepictureObject = JObject.FromObject(new {profilepictureobject= userDataToken.SelectToken("profile_picture").Value<string>() });
                        JObject avatarData = JObject.FromObject(new
                        {
                            profilepicturedata = await client.DownloadDataTaskAsync(new Uri(userDataToken.SelectToken("profile_picture").Value<string>()))
                        });

                        JObject dataObject = JObject.FromObject(new
                        {
                            data = await client.DownloadDataTaskAsync(new Uri(imageObject.SelectToken("url").Value<string>()))
                        });

                        jObject.Merge(imageObject);
                        jObject.Merge(dataObject);
                        jObject.Merge(userObject);
                        jObject.Merge(profilepictureObject);
                        jObject.Merge(avatarData);
                        images.Add(jObject.ToObject<Image>());
                    }

                    imageResponse.Images = images;
                }
            }
            catch (Exception ex)
            {
            }

            return imageResponse;
        }
        
        public async Task<ImageResponse> GetImagesByHashTag(string hashTag, string maxTagId)
        {
            if (string.IsNullOrEmpty(hashTag))
                return await Task.FromResult<ImageResponse>(null);

            string url = string.Format(@"https://api.instagram.com/v1/tags/{0}/media/recent?client_id={1}{2}", 
                hashTag, cClientToken,
                string.IsNullOrEmpty(maxTagId) ? string.Empty : string.Format("&max_tag_id={0}", maxTagId));

            return await GetImagesFromUrl(url);
        }

        public async Task<ImageResponse> GetImagesByUserName(string userName, string minTagId)
        {
            if (string.IsNullOrEmpty(userName))
                return await Task.FromResult<ImageResponse>(null);

            string userSearchUrl = string.Format(@"https://api.instagram.com/v1/users/search?q={0}&client_id={1}", userName, cClientToken);

            ImageResponse imageResponse = null;

            List<Image> images = new List<Image>();
            try
            {
                using (var client = new WebClient())
                {
                    string userData = await client.DownloadStringTaskAsync(new Uri(userSearchUrl));
                    JObject dataToken = JObject.Parse(userData);
                    JToken child = dataToken.SelectToken("data").Children().FirstOrDefault();
                    if (child != null)
                    {
                        string dataRetrieveUrl = string.Format(@"https://api.instagram.com/v1/users/{0}/media/recent?client_id={1}{2}",
                            child.SelectToken("id").Value<string>(),
                            cClientToken,
                            string.IsNullOrEmpty(minTagId) ? string.Empty : string.Format("&min_tag_id={0}", minTagId));

                        return await GetImagesFromUrl(dataRetrieveUrl);
                        string data = await client.DownloadStringTaskAsync(new Uri(dataRetrieveUrl));
                        JObject token = JObject.Parse(data);
                        JObject paginationToken = JObject.FromObject(token.SelectToken("pagination"));
                        imageResponse = paginationToken.ToObject<ImageResponse>();

                        foreach (var tokenData in token.SelectToken("data"))
                        {
                            JObject jObject = JObject.FromObject(new { id = tokenData.SelectToken("id").Value<string>() });
                            JToken imageToken = tokenData.SelectToken("images").SelectToken("standard_resolution");
                            JObject imageObject = JObject.FromObject(imageToken);
                            JObject dataObject = JObject.FromObject(new
                            {
                                data = await client.DownloadDataTaskAsync(new Uri(imageObject.SelectToken("url").Value<string>()))
                            });

                            jObject.Merge(imageObject);
                            jObject.Merge(dataObject);
                            images.Add(jObject.ToObject<Image>());
                        }

                        imageResponse.Images = images;
                        return imageResponse;
                    }
                    else
                    {
                        throw new InvalidOperationException("Пользователь не найден");
                    }
                }
            }
            catch (Exception)
            {
                
            }


            return await Task.FromResult<ImageResponse>(null);
        }
    }
}
