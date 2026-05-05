CREATE TABLE [dbo].[StudentCourseMap] (
    [StudentId]      INT           NOT NULL,
    [CourseId]       INT           NOT NULL,
    [EnrollmentDate] DATETIME2 (7) NOT NULL,
    [CreatedBy]      INT           NOT NULL,
    [CreatedDate]    DATETIME2 (7) NOT NULL,
    [ModifiedBy]     INT           NULL,
    [ModifiedDate]   DATETIME2 (7) NULL,
    [RowVersion]     ROWVERSION    NOT NULL,
    CONSTRAINT [PK_StudentCourseMap] PRIMARY KEY CLUSTERED ([StudentId] ASC, [CourseId] ASC),
    CONSTRAINT [FK_StudentCourseMap_Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentCourseMap_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Student] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentCourseMap_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_StudentCourseMap_User_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[User] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_StudentCourseMap_CourseId]
    ON [dbo].[StudentCourseMap]([CourseId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StudentCourseMap_ModifiedBy]
    ON [dbo].[StudentCourseMap]([ModifiedBy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_StudentCourseMap_CreatedBy]
    ON [dbo].[StudentCourseMap]([CreatedBy] ASC);

