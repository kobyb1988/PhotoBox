using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Monads;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;

namespace ImageMaker.AdminViewModels.Services
{
    public class SessionService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMappingEngine _mappingEngine;

        public SessionService(IImageRepository imageRepository, IMappingEngine mappingEngine)
        {
            _imageRepository = imageRepository;
            _mappingEngine = mappingEngine;
        }

        public virtual void StartSession()
        {
            bool started = _imageRepository.StartSession();
            if (!started)
                return;

            _imageRepository.Commit();
        }

        public virtual bool GetIsSessionActive()
        {
            return _imageRepository.GetActiveSession() != null;
        }

        public virtual string GetLastSessionFolderPath()
        {
            Session activeSession = _imageRepository.GetLastSession();
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (activeSession != null)
                return Path.Combine(Path.Combine(baseDir, "Images"),
                    string.Format("{0}_{1}", activeSession.StartTime.ToString("dd_MM_yyyy"), activeSession.Id));

            return null;
        }

        public virtual void StopSession()
        {
            bool stopped = _imageRepository.StopSession();
            if (!stopped)
                return;

            _imageRepository.Commit();
        }

        public virtual async Task<IEnumerable<CheckableImageWrapper>> GetImagesAsync()
        {
            var session = await _imageRepository.GetLastSessionAsync(true);
            return session.With(c => c.Images.Select(x => new CheckableImageWrapper(_mappingEngine.Map<ImageViewModel>(x))));
        } 
    }
}
