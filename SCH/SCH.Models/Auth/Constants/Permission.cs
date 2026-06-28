namespace SCH.Models.Auth.Constants
{
    public static class Permission
    {
        public const string ClaimType = "permission";

        public static class Student
        {
            public const string Read = "students:read";
            public const string Write = "students:write";
            public const string WriteOwn = "students:write-own";
            public const string Add = "students:add";
            public const string Remove = "students:remove";
        }

        public static class Teacher
        {
            public const string Read = "teachers:read";
            public const string Write = "teachers:write";
            public const string WriteOwn = "teachers:write-own";
            public const string Remove = "teachers:remove";
        }

        public static class Course
        {
            public const string Read = "courses:read";
            public const string Write = "courses:write";
            public const string Add = "courses:add";
            public const string Remove = "courses:remove";
        }
    }

}
