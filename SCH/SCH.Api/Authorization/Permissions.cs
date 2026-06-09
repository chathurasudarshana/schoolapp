namespace SCH.API.Authorization
{
    public static class Permissions
    {
        public const string ClaimType = "permission";

        public static class Students
        {
            public const string Read = "students:read";
            public const string Write = "students:write";
            public const string WriteOwn = "students:write-own";
            public const string Add = "students:add";
        }

        public static class Teachers
        {
            public const string Read = "teachers:read";
            public const string Write = "teachers:write";
            public const string WriteOwn = "teachers:write-own";
        }

        public static class Courses
        {
            public const string Read = "courses:read";
            public const string Write = "courses:write";
            public const string Add = "courses:add";
        }
    }

}
