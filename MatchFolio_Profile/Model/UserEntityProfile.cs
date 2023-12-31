﻿using UtilityLibraries;

namespace MatchFolio_Profile.Model
{
    public class UserEntityProfile : IUser
    {
        public int id { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public DateTime? birthday { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public bool? userType { get; set; }
        public string? cvLink { get; set; }
        public string? linkedinLink { get; set; }
        public string? XLink { get; set; }
        public string? githubLink { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}