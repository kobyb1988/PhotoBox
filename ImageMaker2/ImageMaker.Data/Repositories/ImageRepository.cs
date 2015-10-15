using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Monads;
using System.Threading.Tasks;
using ImageMaker.DataContext.Contexts;
using ImageMaker.Entities;

namespace ImageMaker.Data.Repositories
{
    public interface IImageRepository
    {
        Session GetActiveSession(bool includeImages = false);

        Task<Session> GetActiveSessionAsync(bool includeImages = false);

        bool StartSession();

        IEnumerable<Session> GetAllSessions();
        
        IEnumerable<Image> GetAllImages();

        IEnumerable<Image> GetAllImages(int sessionId);

        void AddImage(Image image);

        void AddImages(IEnumerable<Image> images);

        IEnumerable<Template> GetTemplates();

        Task<IEnumerable<Template>> GetTemplatesAsync();

        IEnumerable<Composition> GetCompositions();

        Task<IEnumerable<Composition>> GetCompositionsAsync();

        void RemoveTemplates(IEnumerable<Template> templates);

        void UpdateTemplates(IEnumerable<Template> templates);

        void UpdateCompositions(IEnumerable<Composition> compositions);

        void RemoveCompositions(IEnumerable<Composition> compositions);

        void AddTemplates(IEnumerable<Template> templates);

        void AddCompositions(IEnumerable<Composition> compositions);

        void Commit();
    }

    public class ImageRepository : RepositoryBase<ImageContext>, IImageRepository
    {
        public ImageRepository(ImageContext context) : base(context)
        {
        }

        public Session GetActiveSession(bool includeImages = false)
        {
            if (includeImages)
                return QueryAll<Session>()
                    .Include(x => x.Images)
                    .FirstOrDefault(x => !x.EndTime.HasValue);

            return this.GetSingle<Session>(x => !x.EndTime.HasValue);
        }

        public async Task<Session> GetActiveSessionAsync(bool includeImages = false)
        {
            if (includeImages)
                return await QueryAll<Session>()
                    .Include(x => x.Images)
                    .FirstOrDefaultAsync(x => !x.EndTime.HasValue);

            return await GetSingleAsync<Session>(x => !x.EndTime.HasValue);
        }

        public bool StartSession()
        {
            Session active = GetActiveSession();
            if (active != null)
                return false;

            active = Context.Set<Session>().Create();
            active.StartTime = DateTime.Now;
            Add(active);
            return true;
        }

        public IEnumerable<Session> GetAllSessions()
        {
            return QueryAll<Session>()
                .Include(x => x.Images)
                .ToList();
        }

        public IEnumerable<Image> GetAllImages()
        {
            return QueryAll<Image>()
                .Include(x => x.Session)
                .ToList();
        }

        public IEnumerable<Image> GetAllImages(int sessionId)
        {
            return QueryAll<Image>()
                .Include(x => x.Session)
                .Where(x => x.SessionId == sessionId)
                .ToList();
        }

        public void AddImage(Image image)
        {
            Add(image);
        }

        public void AddImages(IEnumerable<Image> images)
        {
            Add(images);
        }

        public IEnumerable<Template> GetTemplates()
        {
            return QueryAll<Template>()
                .Include(x => x.Images)
                .Include(x => x.Background)
                .Include(x => x.Overlay).ToList();
        }

        public async Task<IEnumerable<Template>> GetTemplatesAsync()
        {
            return await QueryAll<Template>()
                .Include(x => x.Images)
                .Include(x => x.Overlay)
                .Include(x => x.Background)
                .ToListAsync();
        }

        public IEnumerable<Composition> GetCompositions()
        {
            return null;
            //return QueryAll<Composition>()
            //        .Include(x => x.Template.Images)
            //        .Include(x => x.Overlay.Data)
            //        .Include(x => x.Background.Data)
            //        .ToList();
        }


        public async Task<IEnumerable<Composition>> GetCompositionsAsync()
        {
            return null;
            //return await QueryAll<Composition>()
            //        .Include(x => x.Template.Images)
            //        .Include(x => x.Overlay.Data)
            //        .Include(x => x.Background.Data)
            //        .ToListAsync();
        }

        public void RemoveTemplates(IEnumerable<Template> templates)
        {
            Remove(templates.Select(x => x.Id).Join(QueryAll<Template>(), x => x, x => x.Id, (x, y) => y));
        }

        public void UpdateTemplates(IEnumerable<Template> templates)
        {
            GenericComparer<TemplateImage> comparer = new GenericComparer<TemplateImage>((x, y) => x.Id == y.Id, x => x.Id.GetHashCode());

            foreach (var pair in templates.Join(QueryAll<Template>().Include(x => x.Images), x => x.Id, x => x.Id, (x, y) => new { New = x, Old = y }))
            {
                pair.Old.Height = pair.New.Height;
                pair.Old.Width = pair.New.Width;

                if (pair.Old.BackgroundId != pair.New.BackgroundId)
                    pair.Old.Background = pair.New.Background;

                if (pair.Old.OverlayId != pair.New.OverlayId)
                    pair.Old.Overlay = pair.New.Overlay;

                var removed = pair.Old.Images.Except(pair.New.Images, comparer);
                var added = pair.New.Images.Where(x => x.Id == 0);
                var updated = pair.Old.Images.Join(pair.New.Images, x => x.Id, x => x.Id, (x, y) => new { Old = x, New = y});

                Remove(removed);
                Add(added);
                
                foreach (var image in updated)
                {
                    image.Old.Height = image.New.Height;
                    image.Old.Width = image.New.Width;
                    image.Old.X = image.New.X;
                    image.Old.Y = image.New.Y;
                }
            }
        }

        public void UpdateCompositions(IEnumerable<Composition> compositions)
        {
            //foreach (
            //    var pair in
            //        compositions.Join(QueryAll<Composition>(), x => x.Id, x => x.Id, (x, y) => new {New = x, Old = y}))
            //{
            //    if (pair.Old.BackgroundId != pair.New.BackgroundId)
            //        pair.Old.Background = pair.New.Background;

            //    if (pair.Old.OverlayId != pair.New.OverlayId)
            //        pair.Old.Overlay = pair.New.Overlay;

            //    if (pair.Old.TemplateId != pair.New.TemplateId)
            //        pair.Old.Template = pair.New.Template;
            //}

            //Commit();
        }

        public void RemoveCompositions(IEnumerable<Composition> compositions)
        {
            Remove(compositions.Select(x => x.Id).Join(QueryAll<Composition>(), x => x, x => x.Id, (x, y) => y));
        }

        public void AddTemplates(IEnumerable<Template> templates)
        {
            Add(templates);
        }

        public void AddCompositions(IEnumerable<Composition> compositions)
        {
            Add(compositions);
        }
    }

    public class GenericComparer<TEntity> : IEqualityComparer<TEntity>
    {
        private readonly Func<TEntity, TEntity, bool> _comparer;
        private readonly Func<TEntity, int> _getHashCode;

        public GenericComparer(Func<TEntity, TEntity, bool> comparer, Func<TEntity, int> getHashCode)
        {
            _comparer = comparer;
            _getHashCode = getHashCode;
        }

        public bool Equals(TEntity x, TEntity y)
        {
            return _comparer(x, y);
        }

        public int GetHashCode(TEntity obj)
        {
            return _getHashCode(obj);
        }
    }
}
