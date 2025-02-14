﻿using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Courses_app.Repository;
using Courses_app.WebSocket;
using Microsoft.AspNetCore.SignalR;

namespace Courses_app.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVideoService _videoService;
        private readonly IImageService _imageService;
        private readonly IDailymotionAuthService _auth;
        private readonly IVideoUploadService _videoUploadService;

        public CourseService(ICourseRepository courseRepository, IUserRepository userRepository, IVideoService videoService, IImageService imageService, IDailymotionAuthService auth, IVideoUploadService videoUploadService)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _videoService = videoService;
            _imageService = imageService;
            _auth = auth;
            _videoUploadService = videoUploadService;
        }

        public async Task<long> Add(CreateCourseModel createCourse)
        {
            Author author = await _userRepository.GetAuthorById(createCourse.AuthorId);
            if(author == null)
            {
                throw new Exception();
            }

            string playlistId = await _videoService.CreatePlaylist(createCourse.Name);

            Course course = new Course();
            course.Name = createCourse.Name;
            course.Description = createCourse.Description;
            course.Author = author;
            course.Status = CourseStatus.DRAFT;
            course.PlaylistId = playlistId;

            long courseId = await _courseRepository.Add(course);
            return courseId;

        }

        public async Task<CourseDto> GetAuthorCourse(long authorId, long courseId)
        {
            var course = await _courseRepository.GetAuthorCourse(authorId, courseId);
            if(course == null)
            {
                return null;
            }

            return new CourseDto(course);

        }

        public async Task<List<CourseDto>> GetAuthorCourses(long authorId)
        {
            List<Course> courses = await _courseRepository.GetAuthorCourses(authorId);
            var courseDtos = courses
                .Select(course => new CourseDto(course))
                .ToList();

            return courseDtos;
        }

        //public async Task<CourseDto> AddVideoToCourse(AddVideoModel model)
        //{
        //    Author author = await _userRepository.GetAuthorById(model.authorId);
        //    Course course = await _courseRepository.Get(model.courseId);

        //    string videoId = await _videoService.UploadVideo(model.file.OpenReadStream(), model.title, course.PlaylistId);
        //    Video video = new Video(videoId, author, model.title, model.description, false);

        //    Course updatedCourse = await _courseRepository.AddVideoToCourse(model.courseId, video);
        //    CourseDto courseDto = new CourseDto(updatedCourse);
        //    return courseDto;
        //}

        public async Task<string> AddVideoToCourseAsync(AddVideoModel model)
        {
            Author author = await _userRepository.GetAuthorById(model.authorId);
            Course course = await _courseRepository.Get(model.courseId);

            TokenResponse tokenResponse = await _auth.GetAccessToken();

            var uploadUrlResponse = await _videoService.GetUploadUrl(tokenResponse.Access_token);
            var uploadUrl = uploadUrlResponse.upload_url;

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await model.file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            _videoUploadService.UploadVideoAndAddToCourseAsync(uploadUrl, tokenResponse, model.title, model.description, course, author, fileBytes);

            return uploadUrlResponse.progress_url;
        }

        public async Task<List<VideoDto>> GetCourseVideos(long courseId)
        {
            List<Video> videos = await _courseRepository.GetCourseVideos(courseId);
            var videoDtos = videos
                .Select(video => new VideoDto(video))
                .ToList();

            return videoDtos;
        }

        public async Task<List<CourseDto>> GetAllPublicCourses()
        {
            List<Course> courses = await _courseRepository.GetAllPublicCourses();
            var courseDtos = courses
                .Select(course => new CourseDto(course))
                .ToList();

            return courseDtos;
        }

        public async Task<List<CourseDto>> SearchCourse(string query)
        {
            List<Course> courses = await _courseRepository.SearchCourse(query);
            var courseDtos = courses.Select(course => new CourseDto(course)).ToList();
            return courseDtos;
        }

        public async Task<List<CourseDto>> FilterCourses(FilterDto filter)
        {
            if(filter.Categories == null || (!Enum.TryParse(filter.DifficultyLevel, out DifficultyLevel difficultyLevel) && filter.Categories.Count == 0))
            {
                return await GetAllPublicCourses();
            }
            else
            {

                List<Course> courses = await _courseRepository.FilterCourses(filter);
                var courseDtos = courses
                .Select(course => new CourseDto(course))
                .ToList();

                return courseDtos;
            }
        }

        //public async Task<Course> Get(long id)
        //{
        //    return await _courseRepository.Get(id);
        //}

        public async Task<List<Course>> GetCoursesByIds(List<long> ids)
        {
            return await _courseRepository.GetCoursesByIds(ids);
        }

        //public async Task<CourseDto> GetPurchased(long courseId)
        //{
        //    var course = await _courseRepository.Get(courseId);
        //    CourseDto purchasedCourse = new CourseDto(course);
        //    return purchasedCourse;
        //}

        public async Task<CourseDto> PublishCourse(long courseId, long userId, PublishCourseRequest request)
        {
            if (Enum.TryParse(request.DifficultyLevel, out DifficultyLevel difficultyLevel))
            {
                Course publishedCourse = await _courseRepository.UpdateCourseStatusToPublic(courseId,userId, request.Price, difficultyLevel, request.Categories);
                return new CourseDto(publishedCourse);
            }
            else
            {
                throw new Exception("Invalid difficulty level");
            }
                
        }

        public async Task<CourseDto> ArchiveCourse(long courseId, long userId)
        {
            var course = await _courseRepository.ArchiveCourse(courseId, userId);
            return new CourseDto(course);
        }

        public async Task<Course> UpdateNameAndDescription(long userId, long courseId, string name, string description)
        {
            var course = await _courseRepository.UpdateNameAndDescription(userId, courseId,name,description);
            var playlistId = await _videoService.UpdatePlaylist(course.PlaylistId, name);

            if(course == null)
            {
                return null;
            }

            return course;
        }


        public async Task<Course> AddCourseThumbnail(long authorId, long courseId, IFormFile image)
        {
            try
            {
                string fileName = await _imageService.Upload(image);
                if(fileName == null)
                {
                    throw new BadDataException("An error occured while saving image.");
                }
                return await _courseRepository.AddCourseThumbnail(authorId, courseId, fileName);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<VideoDto> AddVideoThumbnail(long authorId, long courseId, string videoId, IFormFile image)
        {
            try
            {
                string fileName = await _imageService.Upload(image);
                if (fileName == null)
                {
                    throw new BadDataException("An error occured while saving image.");
                }
                var video = await _courseRepository.AddVideoThumbnail(authorId,courseId, videoId, fileName);
                if(video == null)
                {
                    return null;
                }
                return new VideoDto(video);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Video> UpdateVideoPublishedStatus(string videoId)
        {
            var video = await _courseRepository.UpdateVideoPublishedStatus(videoId);
            return video;
        }


    }
}
