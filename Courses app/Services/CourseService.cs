using Courses_app.Dto;
using Courses_app.Exceptions;
using Courses_app.Models;
using Courses_app.Repository;

namespace Courses_app.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVideoService _videoService;

        public CourseService(ICourseRepository courseRepository, IUserRepository userRepository, IVideoService videoService)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _videoService = videoService;
        }

        public async Task<Course> Add(CreateCourseModel createCourse)
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

            Course newCourse = await _courseRepository.Add(course);
            return newCourse;

        }

        public async Task<List<Course>> GetAuthorCourses(long authorId)
        {
            return await _courseRepository.GetAuthorCourses(authorId);
        }

        public async Task<Course> AddVideoToCourse(AddVideoModel model)
        {
            Author author = await _userRepository.GetAuthorById(model.authorId);
            Course course = await _courseRepository.Get(model.courseId);

            string videoId = await _videoService.UploadVideo(model.file.OpenReadStream(), model.title, course.PlaylistId);
            Video video = new Video(videoId, author, model.title, model.description);

            Course updatedCourse = await _courseRepository.AddVideoToCourse(model.courseId, video);
            return updatedCourse;
        }

        public async Task<List<Video>> GetCourseVideos(long courseId)
        {
            return await _courseRepository.GetCourseVideos(courseId);
        }

        public async Task<List<Course>> GetAllPublicCourses()
        {
            return await _courseRepository.GetAllPublicCourses();
        }

        public async Task<Course> Get(long id)
        {
            return await _courseRepository.Get(id);
        }

        public async Task<PurchasedCourse> GetPurchased(long courseId)
        {
            var course = await _courseRepository.Get(courseId);
            PurchasedCourse purchasedCourse = new PurchasedCourse(course);
            return purchasedCourse;
        }

        public async Task<Course> PublishCourse(long courseId, PublishCourseRequest request)
        {
            return await _courseRepository.UpdateCourseStatusToPublic(courseId, request.Price);
        }
    }
}
