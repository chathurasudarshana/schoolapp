CREATE PROCEDURE [dbo].[GetCourseStudentCount]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.Id,
        c.Name,
        COUNT(scm.StudentId) AS StudentCount
    FROM dbo.Course c
    LEFT JOIN dbo.StudentCourseMap scm ON c.Id = scm.CourseId
    GROUP BY c.Id, c.Name
    ORDER BY c.Name;
END
