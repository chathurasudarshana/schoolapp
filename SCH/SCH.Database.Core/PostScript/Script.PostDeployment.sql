/*
Post-Deployment Script - Seed Data							
--------------------------------------------------------------------------------------
 This script seeds Identity (roles, admin user) and domain data (students).
--------------------------------------------------------------------------------------
*/

-- =============================================
-- SECTION 1: Seed Roles
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetRoles] WHERE [Name] = 'Admin')
BEGIN
    INSERT INTO [identity].[AspNetRoles]
    (
        [Name],
        [NormalizedName],
        [ConcurrencyStamp],
        [Description],
        [IsActive],
        [CreatedDate]
    )
    VALUES
    (
        'Admin',
        'ADMIN',
        NEWID(),
        'Administrator role with full access',
        1,
        GETUTCDATE()
    );
    
    PRINT 'Admin role created';
END

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetRoles] WHERE [Name] = 'Basic')
BEGIN
    INSERT INTO [identity].[AspNetRoles]
    (
        [Name],
        [NormalizedName],
        [ConcurrencyStamp],
        [Description],
        [IsActive],
        [CreatedDate]
    )
    VALUES
    (
        'Basic',
        'BASIC',
        NEWID(),
        'Basic user role with limited access',
        1,
        GETUTCDATE()
    );
    
    PRINT 'Basic role created';
END

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetRoles] WHERE [Name] = 'Teacher')
BEGIN
    INSERT INTO [identity].[AspNetRoles]
    (
        [Name],
        [NormalizedName],
        [ConcurrencyStamp],
        [Description],
        [IsActive],
        [CreatedDate]
    )
    VALUES
    (
        'Teacher',
        'TEACHER',
        NEWID(),
        'Teacher role - can add/edit students and courses, read/edit own teacher record',
        1,
        GETUTCDATE()
    );

    PRINT 'Teacher role created';
END

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetRoles] WHERE [Name] = 'Student')
BEGIN
    INSERT INTO [identity].[AspNetRoles]
    (
        [Name],
        [NormalizedName],
        [ConcurrencyStamp],
        [Description],
        [IsActive],
        [CreatedDate]
    )
    VALUES
    (
        'Student',
        'STUDENT',
        NEWID(),
        'Student role - can edit own student record and read courses',
        1,
        GETUTCDATE()
    );

    PRINT 'Student role created';
END

GO

-- =============================================
-- SECTION 1b: Seed Role Claims
-- =============================================

-- Teacher role claims
DECLARE @TeacherRoleId INT = (SELECT [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Teacher');

IF @TeacherRoleId IS NOT NULL
BEGIN
    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'students:read'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'students:read'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'students:write'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'students:write'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'students:add'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'students:add'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'teachers:read'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'teachers:read'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'teachers:write-own'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'teachers:write-own'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'courses:read'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'courses:read'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'courses:write'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'courses:write'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @TeacherRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'courses:add'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @TeacherRoleId, 
            'permission', 
            'courses:add'
        );
    END

    PRINT 'Teacher role claims seeded';
END

-- Student role claims
DECLARE @StudentRoleId INT = (SELECT [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Student');

IF @StudentRoleId IS NOT NULL
BEGIN
    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @StudentRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'students:read'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @StudentRoleId, 
            'permission', 
            'students:read'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @StudentRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'students:write-own'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @StudentRoleId, 
            'permission', 
            'students:write-own'
        );
    END

    IF NOT EXISTS 
    (
        SELECT 1 
        FROM [identity].[AspNetRoleClaims] 
        WHERE [RoleId] = @StudentRoleId AND [ClaimType] = 'permission' AND [ClaimValue] = 'courses:read'
    )
    BEGIN
        INSERT INTO [identity].[AspNetRoleClaims] 
        (
            [RoleId], 
            [ClaimType], 
            [ClaimValue]
        ) 
        VALUES 
        (
            @StudentRoleId, 
            'permission', 
            'courses:read'
        );
    END

    PRINT 'Student role claims seeded';
END

GO

-- =============================================
-- SECTION 2: Seed Admin User
-- =============================================
-- Password: Admin123!
-- This is a default password - CHANGE IT IN PRODUCTION!

DECLARE @AdminUserId INT;
DECLARE @AdminRoleId INT;

-- Check if admin user already exists
IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'admin')
BEGIN
    -- Insert Admin User
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'admin',                                                                    -- UserName
        'ADMIN',                                                                    -- NormalizedUserName
        'admin@schoolapp.com',                                                      -- Email
        'ADMIN@SCHOOLAPP.COM',                                                      -- NormalizedEmail
        1,                                                                          -- EmailConfirmed
        'AQAAAAIAAYagAAAAEKkmWMV2APYtFbk6tjDGmXF8mLC0ScPolspmV06LnuJxcxnzJMupu6OUp7txvNIDgQ==', -- PasswordHash for "Admin123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),                                                                    -- SecurityStamp
        NEWID(),                                                                    -- ConcurrencyStamp
        NULL,                                                                       -- PhoneNumber
        0,                                                                          -- PhoneNumberConfirmed
        0,                                                                          -- TwoFactorEnabled
        NULL,                                                                       -- LockoutEnd
        1,                                                                          -- LockoutEnabled
        0,                                                                          -- AccessFailedCount
        'System',                                                                   -- FirstName
        'Administrator',                                                            -- LastName
        1,                                                                          -- IsActive
        GETUTCDATE(),                                                               -- CreatedDate
        NULL,                                                                       -- LastLoginDate
        NULL,                                                                       -- CreatedBy (self-reference, first user)
        NULL,                                                                       -- ModifiedBy
        NULL                                                                        -- ModifiedDate
    );
    
    SET @AdminUserId = SCOPE_IDENTITY();
    PRINT 'Admin user created with UserId: ' + CAST(@AdminUserId AS NVARCHAR(10));
    
    -- Get Admin Role Id
    SELECT @AdminRoleId = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Admin';
    
    -- Assign Admin role to Admin user
    IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUserRoles] WHERE [UserId] = @AdminUserId AND [RoleId] = @AdminRoleId)
    BEGIN
        INSERT INTO [identity].[AspNetUserRoles]
        (
            [UserId],
            [RoleId]
        )
        VALUES
        (
            @AdminUserId,
            @AdminRoleId
        );
        
        PRINT 'Admin role assigned to admin user';
    END
    
    -- Create corresponding domain user record
    IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Id] = @AdminUserId)
    BEGIN
        INSERT INTO [dbo].[User]
        (
            [Id],
            [FirstName],
            [LastName],
            [CreatedBy],
            [CreatedDate],
            [ModifiedBy],
            [ModifiedDate]
        )
        VALUES
        (
            @AdminUserId,
            'System',
            'Administrator',
            NULL,           -- CreatedBy (self-reference, first user)
            GETUTCDATE(),   -- CreatedDate
            NULL,           -- ModifiedBy
            NULL            -- ModifiedDate
        );
        
        PRINT 'Domain user record created for admin';
    END
END
ELSE
BEGIN
    PRINT 'Admin user already exists';
END

GO

-- =============================================
-- SECTION 3: Seed Basic User
-- =============================================
-- Password: Basic123!
-- This is a default password - CHANGE IT IN PRODUCTION!

DECLARE @BasicUserId INT;
DECLARE @BasicRoleId INT;

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'basicuser')
BEGIN
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'basicuser',                                                                -- UserName
        'BASICUSER',                                                                -- NormalizedUserName
        'basicuser@schoolapp.com',                                                  -- Email
        'BASICUSER@SCHOOLAPP.COM',                                                  -- NormalizedEmail
        1,                                                                          -- EmailConfirmed
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),                                                                    -- SecurityStamp
        NEWID(),                                                                    -- ConcurrencyStamp
        NULL,                                                                       -- PhoneNumber
        0,                                                                          -- PhoneNumberConfirmed
        0,                                                                          -- TwoFactorEnabled
        NULL,                                                                       -- LockoutEnd
        1,                                                                          -- LockoutEnabled
        0,                                                                          -- AccessFailedCount
        'Basic',                                                                    -- FirstName
        'User',                                                                     -- LastName
        1,                                                                          -- IsActive
        GETUTCDATE(),                                                               -- CreatedDate
        NULL,                                                                       -- LastLoginDate
        NULL,                                                                       -- CreatedBy
        NULL,                                                                       -- ModifiedBy
        NULL                                                                        -- ModifiedDate
    );

    SET @BasicUserId = SCOPE_IDENTITY();
    PRINT 'Basic user created with UserId: ' + CAST(@BasicUserId AS NVARCHAR(10));

    -- Get Basic Role Id
    SELECT @BasicRoleId = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Basic';

    -- Assign Basic role to Basic user
    IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUserRoles] WHERE [UserId] = @BasicUserId AND [RoleId] = @BasicRoleId)
    BEGIN
        INSERT INTO [identity].[AspNetUserRoles]
        (
            [UserId],
            [RoleId]
        )
        VALUES
        (
            @BasicUserId,
            @BasicRoleId
        );

        PRINT 'Basic role assigned to basic user';
    END

    -- Create corresponding domain user record
    IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Id] = @BasicUserId)
    BEGIN
        INSERT INTO [dbo].[User]
        (
            [Id],
            [FirstName],
            [LastName],
            [CreatedBy],
            [CreatedDate],
            [ModifiedBy],
            [ModifiedDate]
        )
        VALUES
        (
            @BasicUserId,
            'Basic',
            'User',
            NULL,           -- CreatedBy
            GETUTCDATE(),   -- CreatedDate
            NULL,           -- ModifiedBy
            NULL            -- ModifiedDate
        );

        PRINT 'Domain user record created for basic user';
    END
END
ELSE
BEGIN
    PRINT 'Basic user already exists';
END

GO

-- =============================================
-- SECTION 4: Seed Teacher Users
-- =============================================
-- Password: Basic123!

DECLARE @Teacher1UserId INT;
DECLARE @Teacher2UserId INT;
DECLARE @TeacherRoleIdForUsers INT;
DECLARE @BasicRoleIdForTeachers INT;

SELECT @TeacherRoleIdForUsers = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Teacher';
SELECT @BasicRoleIdForTeachers = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Basic';

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'teacher1')
BEGIN
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'teacher1',
        'TEACHER1',
        'teacher1@schoolapp.com',
        'TEACHER1@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'Teacher',
        'One',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @Teacher1UserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Teacher1UserId, @BasicRoleIdForTeachers);

    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId])
    VALUES (@Teacher1UserId, @TeacherRoleIdForUsers);

    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @Teacher1UserId,
        'Teacher',
        'One',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'teacher1 created';
END

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'teacher2')
BEGIN
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'teacher2',
        'TEACHER2',
        'teacher2@schoolapp.com',
        'TEACHER2@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'Teacher',
        'Two',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @Teacher2UserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Teacher2UserId, @BasicRoleIdForTeachers);
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Teacher2UserId, @TeacherRoleIdForUsers);

    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @Teacher2UserId,
        'Teacher',
        'Two',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'teacher2 created';
END

GO

-- =============================================
-- SECTION 5: Seed Student Users
-- =============================================
-- Password: Basic123!

DECLARE @Student1UserId INT;
DECLARE @Student2UserId INT;
DECLARE @StudentRoleIdForUsers INT;
DECLARE @BasicRoleIdForStudents INT;

SELECT @StudentRoleIdForUsers = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Student';
SELECT @BasicRoleIdForStudents = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Basic';

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'student1')
BEGIN
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'student1',
        'STUDENT1',
        'student1@schoolapp.com',
        'STUDENT1@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'Student',
        'One',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @Student1UserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Student1UserId, @BasicRoleIdForStudents);
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Student1UserId, @StudentRoleIdForUsers);
    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @Student1UserId,
        'Student',
        'One',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'student1 created';
END

IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'student2')
BEGIN
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'student2',
        'STUDENT2',
        'student2@schoolapp.com',
        'STUDENT2@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'Student',
        'Two',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @Student2UserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Student2UserId, @BasicRoleIdForStudents);
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@Student2UserId, @StudentRoleIdForUsers);
    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @Student2UserId,
        'Student',
        'Two',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'student2 created';
END

GO

-- =============================================
-- SECTION 6: Seed Special Basic View Users
-- =============================================
-- These users have Basic role + a single view-only claim.
-- Password: Basic123!

DECLARE @BasicRoleIdForViewers INT;
SELECT @BasicRoleIdForViewers = [Id] FROM [identity].[AspNetRoles] WHERE [Name] = 'Basic';

-- TeacherViewUser: can only view teachers page
IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'teacherviewuser')
BEGIN
    DECLARE @TeacherViewUserId INT;
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'teacherviewuser',
        'TEACHERVIEWUSER',
        'teacherviewuser@schoolapp.com',
        'TEACHERVIEWUSER@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'TeacherView',
        'User',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @TeacherViewUserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@TeacherViewUserId, @BasicRoleIdForViewers);
    INSERT INTO [identity].[AspNetUserClaims] ([UserId],[ClaimType],[ClaimValue]) 
    VALUES (@TeacherViewUserId, 'permission', 'teachers:read');
    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @TeacherViewUserId,
        'TeacherView',
        'User',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'teacherviewuser created';
END

-- CourseViewUser: can only view courses page
IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'courseviewuser')
BEGIN
    DECLARE @CourseViewUserId INT;
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'courseviewuser',
        'COURSEVIEWUSER',
        'courseviewuser@schoolapp.com',
        'COURSEVIEWUSER@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'CourseView',
        'User',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @CourseViewUserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@CourseViewUserId, @BasicRoleIdForViewers);
    INSERT INTO [identity].[AspNetUserClaims] ([UserId],[ClaimType],[ClaimValue]) 
    VALUES (@CourseViewUserId, 'permission', 'courses:read');
    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @CourseViewUserId,
        'CourseView',
        'User',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'courseviewuser created';
END

-- StudentViewUser: can only view students page
IF NOT EXISTS (SELECT 1 FROM [identity].[AspNetUsers] WHERE [UserName] = 'studentviewuser')
BEGIN
    DECLARE @StudentViewUserId INT;
    INSERT INTO [identity].[AspNetUsers]
    (
        [UserName],
        [NormalizedUserName],
        [Email],
        [NormalizedEmail],
        [EmailConfirmed],
        [PasswordHash],
        [SecurityStamp],
        [ConcurrencyStamp],
        [PhoneNumber],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [LockoutEnd],
        [LockoutEnabled],
        [AccessFailedCount],
        [FirstName],
        [LastName],
        [IsActive],
        [CreatedDate],
        [LastLoginDate],
        [CreatedBy],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'studentviewuser',
        'STUDENTVIEWUSER',
        'studentviewuser@schoolapp.com',
        'STUDENTVIEWUSER@SCHOOLAPP.COM',
        1,
        'AQAAAAIAAYagAAAAEIGSV6dcLldguuvqLg4nEDB8keDYvX5ahRVABWljBJVJngrbWoC9HxCZl6fnLh+EWw==', -- PasswordHash for "Basic123!" (generated with ASP.NET Identity PasswordHasher)Ww==',
        NEWID(),
        NEWID(),
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        'StudentView',
        'User',
        1,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        NULL
    );
    SET @StudentViewUserId = SCOPE_IDENTITY();
    INSERT INTO [identity].[AspNetUserRoles] ([UserId],[RoleId]) 
    VALUES (@StudentViewUserId, @BasicRoleIdForViewers);
    INSERT INTO [identity].[AspNetUserClaims] ([UserId],[ClaimType],[ClaimValue]) 
    VALUES (@StudentViewUserId, 'permission', 'students:read');
    INSERT INTO [dbo].[User] 
    (
        [Id],
        [FirstName],
        [LastName],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES 
    (
        @StudentViewUserId,
        'StudentView',
        'User',
        NULL,
        GETUTCDATE(),
        NULL,
        NULL
    );
    PRINT 'studentviewuser created';
END

GO

-- =============================================
-- SECTION 7: Seed Teacher Records
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Teacher])
BEGIN
    DECLARE @AdminUserIdForTeachers INT = (SELECT TOP 1 [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'admin');
    DECLARE @Teacher1UserIdForRecord INT = (SELECT [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'teacher1');
    DECLARE @Teacher2UserIdForRecord INT = (SELECT [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'teacher2');

    INSERT INTO [dbo].[Teacher] 
    (
        [Name],
        [UserId],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
        (
            'Teacher One', 
            @Teacher1UserIdForRecord, 
            @AdminUserIdForTeachers, 
            GETUTCDATE(), 
            NULL, 
            NULL
        ),
        (
            'Teacher Two', 
            @Teacher2UserIdForRecord, 
            @AdminUserIdForTeachers, 
            GETUTCDATE(), 
            NULL, 
            NULL
        );

    PRINT 'Teacher records seeded';
END

GO

-- =============================================
-- SECTION 8: Seed Student Mock Data
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Student])
BEGIN
    DECLARE @AdminUserIdForStudents INT = (SELECT TOP 1 [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'admin');
    DECLARE @Student1UserIdForRecord INT = (SELECT [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'student1');
    DECLARE @Student2UserIdForRecord INT = (SELECT [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'student2');

    INSERT INTO [dbo].[Student]
    (
        [FirstName],
        [LastName],
        [Email],
        [PhoneNumber],
        [SSN],
        [Image],
        [StartDate],
        [IsActive],
        [UserId],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    (
        'FirstName1',
        'LastName1',
        'email1@mail.com',
        'phonenumber1',
        'ssn1',
        'image1.png',
        '2024-11-11',
        1,
        @Student1UserIdForRecord,
        @AdminUserIdForStudents,
        GETUTCDATE(),
        NULL,
        NULL
    ),
    (
        'FirstName2',
        'LastName2',
        'email2@mail.com',
        'phonenumber2',
        'ssn2',
        'image2.png',
        '2024-11-12',
        1,
        @Student2UserIdForRecord,
        @AdminUserIdForStudents,
        GETUTCDATE(),
        NULL,
        NULL
    ),
    (
        'FirstName3',
        'LastName3',
        'email3@mail.com',
        'phonenumber3',
        'ssn3',
        'image3.png',
        '2024-11-13',
        1,
        NULL,
        @AdminUserIdForStudents,
        GETUTCDATE(),
        NULL,
        NULL
    ),
    (
        'FirstName4',
        'LastName4',
        'email4@mail.com',
        'phonenumber4',
        'ssn4',
        'image4.png',
        '2024-11-14',
        1,
        NULL,
        @AdminUserIdForStudents,
        GETUTCDATE(),
        NULL,
        NULL
    ),
    (
        'FirstName5',
        'LastName5',
        'email5@mail.com',
        'phonenumber5',
        'ssn5',
        'image5.png',
        '2024-11-15',
        0,
        NULL,
        @AdminUserIdForStudents,
        GETUTCDATE(),
        NULL,
        NULL
    ),
    (
        'FirstName6',
        'LastName6',
        'email6@mail.com',
        'phonenumber6',
        'ssn6',
        'image6.png',
        '2024-11-16',
        1,
        NULL,
        @AdminUserIdForStudents,
        GETUTCDATE(),
        NULL,
        NULL
    )

END

GO

-- =============================================
-- SECTION 9: Seed Course Records
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Course])
BEGIN
    DECLARE @AdminUserIdForCourses INT = (SELECT TOP 1 [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'admin');

    INSERT INTO [dbo].[Course]
    (
        [Name],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    ('Mathematics', @AdminUserIdForCourses, GETUTCDATE(), NULL, NULL),
    ('Science',     @AdminUserIdForCourses, GETUTCDATE(), NULL, NULL),
    ('English',     @AdminUserIdForCourses, GETUTCDATE(), NULL, NULL),
    ('History',     @AdminUserIdForCourses, GETUTCDATE(), NULL, NULL); -- no students enrolled

    PRINT 'Course records seeded';
END

GO

-- =============================================
-- SECTION 10: Seed Student-Course Mappings
-- (History is intentionally left with no students)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[StudentCourseMap])
BEGIN
    DECLARE @AdminUserIdForMaps INT = (SELECT TOP 1 [Id] FROM [identity].[AspNetUsers] WHERE [UserName] = 'admin');

    DECLARE @MathId    INT = (SELECT [Id] FROM [dbo].[Course] WHERE [Name] = 'Mathematics');
    DECLARE @ScienceId INT = (SELECT [Id] FROM [dbo].[Course] WHERE [Name] = 'Science');
    DECLARE @EnglishId INT = (SELECT [Id] FROM [dbo].[Course] WHERE [Name] = 'English');

    DECLARE @S1Id INT = (SELECT [Id] FROM [dbo].[Student] WHERE [Email] = 'email1@mail.com');
    DECLARE @S2Id INT = (SELECT [Id] FROM [dbo].[Student] WHERE [Email] = 'email2@mail.com');
    DECLARE @S3Id INT = (SELECT [Id] FROM [dbo].[Student] WHERE [Email] = 'email3@mail.com');
    DECLARE @S4Id INT = (SELECT [Id] FROM [dbo].[Student] WHERE [Email] = 'email4@mail.com');
    DECLARE @S5Id INT = (SELECT [Id] FROM [dbo].[Student] WHERE [Email] = 'email5@mail.com');

    IF @S1Id IS NULL OR @S2Id IS NULL OR @S3Id IS NULL OR @S4Id IS NULL OR @S5Id IS NULL
    BEGIN
        PRINT 'Skipping Student-Course mappings: one or more expected student records not found';
    END
    ELSE
    BEGIN
    INSERT INTO [dbo].[StudentCourseMap]
    (
        [StudentId],
        [CourseId],
        [EnrollmentDate],
        [CreatedBy],
        [CreatedDate],
        [ModifiedBy],
        [ModifiedDate]
    )
    VALUES
    -- Student 1: Mathematics + Science
    (@S1Id, @MathId,    '2024-11-11', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    (@S1Id, @ScienceId, '2024-11-11', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    -- Student 2: Mathematics + English
    (@S2Id, @MathId,    '2024-11-12', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    (@S2Id, @EnglishId, '2024-11-12', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    -- Student 3: Mathematics only
    (@S3Id, @MathId,    '2024-11-13', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    -- Student 4: Mathematics + Science + English
    (@S4Id, @MathId,    '2024-11-14', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    (@S4Id, @ScienceId, '2024-11-14', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    (@S4Id, @EnglishId, '2024-11-14', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL),
    -- Student 5: English only
    (@S5Id, @EnglishId, '2024-11-15', @AdminUserIdForMaps, GETUTCDATE(), NULL, NULL);

    PRINT 'Student-Course mappings seeded';
    END -- end ELSE
END

GO