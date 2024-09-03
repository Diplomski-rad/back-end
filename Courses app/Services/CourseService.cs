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

        public async Task<List<CourseDto>> GetAuthorCourses(long authorId)
        {
            List<Course> courses = await _courseRepository.GetAuthorCourses(authorId);
            var courseDtos = courses
                .Select(course => new CourseDto(course))
                .ToList();

            return courseDtos;
        }

        public async Task<CourseDto> AddVideoToCourse(AddVideoModel model)
        {
            Author author = await _userRepository.GetAuthorById(model.authorId);
            Course course = await _courseRepository.Get(model.courseId);

            string videoId = await _videoService.UploadVideo(model.file.OpenReadStream(), model.title, course.PlaylistId);
            Video video = new Video(videoId, author, model.title, model.description);

            Course updatedCourse = await _courseRepository.AddVideoToCourse(model.courseId, video);
            CourseDto courseDto = new CourseDto(updatedCourse);
            return courseDto;
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

        public async Task<Course> Get(long id)
        {
            return await _courseRepository.Get(id);
        }

        public async Task<List<Course>> GetCoursesByIds(List<long> ids)
        {
            return await _courseRepository.GetCoursesByIds(ids);
        }

        public async Task<CourseDto> GetPurchased(long courseId)
        {
            var course = await _courseRepository.Get(courseId);
            CourseDto purchasedCourse = new CourseDto(course);
            return purchasedCourse;
        }

        public async Task<CourseDto> PublishCourse(long courseId, PublishCourseRequest request)
        {
            Course publishedCourse = await _courseRepository.UpdateCourseStatusToPublic(courseId, request.Price);
            return new CourseDto(publishedCourse);
        }

        
    }
}
