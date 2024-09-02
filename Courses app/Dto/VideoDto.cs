﻿using Courses_app.Models;

namespace Courses_app.Dto
{
    public class VideoDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public VideoDto(Video video)
        {
            Id = video.Id;
            Title = video.Title;
            Description = video.Description;
        }
    }
}
