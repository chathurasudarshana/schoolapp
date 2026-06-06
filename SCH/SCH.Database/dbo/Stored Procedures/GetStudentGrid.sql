CREATE PROCEDURE [dbo].[GetStudentGrid]
    @PageNumber          INT,
    @PageSize            INT,
    @SortBy              NVARCHAR(100)  = NULL,
    @SortByOperator      NVARCHAR(10)   = NULL,   -- 'asc' | 'desc'
    @FirstNameValue1                  NVARCHAR(400) = NULL,
    @FirstNameOperator1               NVARCHAR(20)  = NULL,
    @FirstNameValue2                  NVARCHAR(400) = NULL,
    @FirstNameOperator2               NVARCHAR(20)  = NULL,
    @FirstNameFilterConcatOperator    NVARCHAR(5)   = NULL,   -- 'AND' | 'OR'
    @LastNameValue1                   NVARCHAR(400) = NULL,
    @LastNameOperator1                NVARCHAR(20)  = NULL,
    @LastNameValue2                   NVARCHAR(400) = NULL,
    @LastNameOperator2                NVARCHAR(20)  = NULL,
    @LastNameFilterConcatOperator     NVARCHAR(5)   = NULL,   -- 'AND' | 'OR'
    @EmailValue1                      NVARCHAR(400) = NULL,
    @EmailOperator1                   NVARCHAR(20)  = NULL,
    @EmailValue2                      NVARCHAR(400) = NULL,
    @EmailOperator2                   NVARCHAR(20)  = NULL,
    @EmailFilterConcatOperator        NVARCHAR(5)   = NULL,   -- 'AND' | 'OR'
    @PhoneNumberValue1                NVARCHAR(20)  = NULL,
    @PhoneNumberOperator1             NVARCHAR(20)  = NULL,
    @PhoneNumberValue2                NVARCHAR(20)  = NULL,
    @PhoneNumberOperator2             NVARCHAR(20)  = NULL,
    @PhoneNumberFilterConcatOperator  NVARCHAR(5)   = NULL,   -- 'AND' | 'OR'
    @SSNValue1                        NVARCHAR(20)  = NULL,
    @SSNOperator1                     NVARCHAR(20)  = NULL,
    @SSNValue2                        NVARCHAR(20)  = NULL,
    @SSNOperator2                     NVARCHAR(20)  = NULL,
    @SSNFilterConcatOperator          NVARCHAR(5)   = NULL,   -- 'AND' | 'OR'
    @StartDateOperator1               NVARCHAR(20)  = NULL,   -- ag-grid operator name
    @StartDateValue1                  NVARCHAR(30)  = NULL,   -- dateFrom (ISO date string)
    @StartDateValue2                  NVARCHAR(30)  = NULL,   -- dateTo   (inRange only)
    @StartDateFilterConcatOperator    NVARCHAR(5)   = NULL,   -- 'AND' | 'OR'
    @StartDateOperator2               NVARCHAR(20)  = NULL,
    @StartDateValue3                  NVARCHAR(30)  = NULL,   -- dateFrom for cond 2
    @StartDateValue4                  NVARCHAR(30)  = NULL,   -- dateTo   for cond 2 (inRange only)
    @IsActive            BIT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT
        s.Id,
        s.FirstName,
        s.LastName,
        s.Email,
        s.PhoneNumber,
        s.SSN,
        s.Image,
        s.StartDate,
        s.IsActive,
        s.CreatedBy,
        s.CreatedDate,
        s.ModifiedBy,
        s.ModifiedDate,
        s.RowVersion,
        COUNT(*) OVER() AS TotalCount
    FROM [dbo].[Student] s
    WHERE
        -- FirstName filter
        (
            @FirstNameOperator1 IS NULL
            OR 
            (
                @FirstNameOperator2 IS NULL
                AND 
                (
                    (@FirstNameOperator1 = 'equals'      AND s.FirstName =          @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'notEqual'    AND s.FirstName <>       @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'contains'    AND s.FirstName LIKE '%' + @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'notContains' AND s.FirstName NOT LIKE '%' + @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'startsWith'  AND s.FirstName LIKE     @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'endsWith'    AND s.FirstName LIKE '%' + @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'blank'       AND (s.FirstName IS NULL OR s.FirstName = ''))
                    OR (@FirstNameOperator1 = 'notBlank'    AND (s.FirstName IS NOT NULL AND s.FirstName <> ''))
                )
            )
            OR 
            (
                @FirstNameFilterConcatOperator = 'OR'
                AND 
                (
                    (@FirstNameOperator1 = 'equals'      AND s.FirstName =          @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'notEqual'    AND s.FirstName <>       @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'contains'    AND s.FirstName LIKE '%' + @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'notContains' AND s.FirstName NOT LIKE '%' + @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'startsWith'  AND s.FirstName LIKE     @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'endsWith'    AND s.FirstName LIKE '%' + @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'blank'       AND (s.FirstName IS NULL OR s.FirstName = ''))
                    OR (@FirstNameOperator1 = 'notBlank'    AND (s.FirstName IS NOT NULL AND s.FirstName <> ''))
                    OR (@FirstNameOperator2 = 'equals'      AND s.FirstName =          @FirstNameValue2)
                    OR (@FirstNameOperator2 = 'notEqual'    AND s.FirstName <>       @FirstNameValue2)
                    OR (@FirstNameOperator2 = 'contains'    AND s.FirstName LIKE '%' + @FirstNameValue2 + '%')
                    OR (@FirstNameOperator2 = 'notContains' AND s.FirstName NOT LIKE '%' + @FirstNameValue2 + '%')
                    OR (@FirstNameOperator2 = 'startsWith'  AND s.FirstName LIKE     @FirstNameValue2 + '%')
                    OR (@FirstNameOperator2 = 'endsWith'    AND s.FirstName LIKE '%' + @FirstNameValue2)
                    OR (@FirstNameOperator2 = 'blank'       AND (s.FirstName IS NULL OR s.FirstName = ''))
                    OR (@FirstNameOperator2 = 'notBlank'    AND (s.FirstName IS NOT NULL AND s.FirstName <> ''))
                )
            )
            OR 
            (
                @FirstNameFilterConcatOperator = 'AND'
                AND 
                (
                    (@FirstNameOperator1 = 'equals'      AND s.FirstName =          @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'notEqual'    AND s.FirstName <>       @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'contains'    AND s.FirstName LIKE '%' + @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'notContains' AND s.FirstName NOT LIKE '%' + @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'startsWith'  AND s.FirstName LIKE     @FirstNameValue1 + '%')
                    OR (@FirstNameOperator1 = 'endsWith'    AND s.FirstName LIKE '%' + @FirstNameValue1)
                    OR (@FirstNameOperator1 = 'blank'       AND (s.FirstName IS NULL OR s.FirstName = ''))
                    OR (@FirstNameOperator1 = 'notBlank'    AND (s.FirstName IS NOT NULL AND s.FirstName <> ''))
                )
                AND 
                (
                    (@FirstNameOperator2 = 'equals'      AND s.FirstName =          @FirstNameValue2)
                    OR (@FirstNameOperator2 = 'notEqual'    AND s.FirstName <>       @FirstNameValue2)
                    OR (@FirstNameOperator2 = 'contains'    AND s.FirstName LIKE '%' + @FirstNameValue2 + '%')
                    OR (@FirstNameOperator2 = 'notContains' AND s.FirstName NOT LIKE '%' + @FirstNameValue2 + '%')
                    OR (@FirstNameOperator2 = 'startsWith'  AND s.FirstName LIKE     @FirstNameValue2 + '%')
                    OR (@FirstNameOperator2 = 'endsWith'    AND s.FirstName LIKE '%' + @FirstNameValue2)
                    OR (@FirstNameOperator2 = 'blank'       AND (s.FirstName IS NULL OR s.FirstName = ''))
                    OR (@FirstNameOperator2 = 'notBlank'    AND (s.FirstName IS NOT NULL AND s.FirstName <> ''))
                )
            )
        )
        -- LastName filter
        AND 
        (
            @LastNameOperator1 IS NULL
            OR 
            (
                @LastNameOperator2 IS NULL
                AND 
                (
                    (@LastNameOperator1 = 'equals'      AND s.LastName =          @LastNameValue1)
                    OR (@LastNameOperator1 = 'notEqual'    AND s.LastName <>       @LastNameValue1)
                    OR (@LastNameOperator1 = 'contains'    AND s.LastName LIKE '%' + @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'notContains' AND s.LastName NOT LIKE '%' + @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'startsWith'  AND s.LastName LIKE     @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'endsWith'    AND s.LastName LIKE '%' + @LastNameValue1)
                    OR (@LastNameOperator1 = 'blank'       AND (s.LastName IS NULL OR s.LastName = ''))
                    OR (@LastNameOperator1 = 'notBlank'    AND (s.LastName IS NOT NULL AND s.LastName <> ''))
                )
            )
            OR 
            (
                @LastNameFilterConcatOperator = 'OR'
                AND 
                (
                    (@LastNameOperator1 = 'equals'      AND s.LastName =          @LastNameValue1)
                    OR (@LastNameOperator1 = 'notEqual'    AND s.LastName <>       @LastNameValue1)
                    OR (@LastNameOperator1 = 'contains'    AND s.LastName LIKE '%' + @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'notContains' AND s.LastName NOT LIKE '%' + @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'startsWith'  AND s.LastName LIKE     @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'endsWith'    AND s.LastName LIKE '%' + @LastNameValue1)
                    OR (@LastNameOperator1 = 'blank'       AND (s.LastName IS NULL OR s.LastName = ''))
                    OR (@LastNameOperator1 = 'notBlank'    AND (s.LastName IS NOT NULL AND s.LastName <> ''))
                    OR (@LastNameOperator2 = 'equals'      AND s.LastName =          @LastNameValue2)
                    OR (@LastNameOperator2 = 'notEqual'    AND s.LastName <>       @LastNameValue2)
                    OR (@LastNameOperator2 = 'contains'    AND s.LastName LIKE '%' + @LastNameValue2 + '%')
                    OR (@LastNameOperator2 = 'notContains' AND s.LastName NOT LIKE '%' + @LastNameValue2 + '%')
                    OR (@LastNameOperator2 = 'startsWith'  AND s.LastName LIKE     @LastNameValue2 + '%')
                    OR (@LastNameOperator2 = 'endsWith'    AND s.LastName LIKE '%' + @LastNameValue2)
                    OR (@LastNameOperator2 = 'blank'       AND (s.LastName IS NULL OR s.LastName = ''))
                    OR (@LastNameOperator2 = 'notBlank'    AND (s.LastName IS NOT NULL AND s.LastName <> ''))
                )
            )
            OR 
            (
                @LastNameFilterConcatOperator = 'AND'
                AND 
                (
                    (@LastNameOperator1 = 'equals'      AND s.LastName =          @LastNameValue1)
                    OR (@LastNameOperator1 = 'notEqual'    AND s.LastName <>       @LastNameValue1)
                    OR (@LastNameOperator1 = 'contains'    AND s.LastName LIKE '%' + @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'notContains' AND s.LastName NOT LIKE '%' + @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'startsWith'  AND s.LastName LIKE     @LastNameValue1 + '%')
                    OR (@LastNameOperator1 = 'endsWith'    AND s.LastName LIKE '%' + @LastNameValue1)
                    OR (@LastNameOperator1 = 'blank'       AND (s.LastName IS NULL OR s.LastName = ''))
                    OR (@LastNameOperator1 = 'notBlank'    AND (s.LastName IS NOT NULL AND s.LastName <> ''))
                )
                AND 
                (
                    (@LastNameOperator2 = 'equals'      AND s.LastName =          @LastNameValue2)
                    OR (@LastNameOperator2 = 'notEqual'    AND s.LastName <>       @LastNameValue2)
                    OR (@LastNameOperator2 = 'contains'    AND s.LastName LIKE '%' + @LastNameValue2 + '%')
                    OR (@LastNameOperator2 = 'notContains' AND s.LastName NOT LIKE '%' + @LastNameValue2 + '%')
                    OR (@LastNameOperator2 = 'startsWith'  AND s.LastName LIKE     @LastNameValue2 + '%')
                    OR (@LastNameOperator2 = 'endsWith'    AND s.LastName LIKE '%' + @LastNameValue2)
                    OR (@LastNameOperator2 = 'blank'       AND (s.LastName IS NULL OR s.LastName = ''))
                    OR (@LastNameOperator2 = 'notBlank'    AND (s.LastName IS NOT NULL AND s.LastName <> ''))
                )
            )
        )
        -- Email filter
        AND 
        (
            @EmailOperator1 IS NULL
            OR 
            (
                @EmailOperator2 IS NULL
                AND 
                (
                    (@EmailOperator1 = 'equals'      AND s.Email =          @EmailValue1)
                    OR (@EmailOperator1 = 'notEqual'    AND s.Email <>       @EmailValue1)
                    OR (@EmailOperator1 = 'contains'    AND s.Email LIKE '%' + @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'notContains' AND s.Email NOT LIKE '%' + @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'startsWith'  AND s.Email LIKE     @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'endsWith'    AND s.Email LIKE '%' + @EmailValue1)
                    OR (@EmailOperator1 = 'blank'       AND (s.Email IS NULL OR s.Email = ''))
                    OR (@EmailOperator1 = 'notBlank'    AND (s.Email IS NOT NULL AND s.Email <> ''))
                )
            )
            OR 
            (
                @EmailFilterConcatOperator = 'OR'
                AND 
                (
                    (@EmailOperator1 = 'equals'      AND s.Email =          @EmailValue1)
                    OR (@EmailOperator1 = 'notEqual'    AND s.Email <>       @EmailValue1)
                    OR (@EmailOperator1 = 'contains'    AND s.Email LIKE '%' + @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'notContains' AND s.Email NOT LIKE '%' + @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'startsWith'  AND s.Email LIKE     @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'endsWith'    AND s.Email LIKE '%' + @EmailValue1)
                    OR (@EmailOperator1 = 'blank'       AND (s.Email IS NULL OR s.Email = ''))
                    OR (@EmailOperator1 = 'notBlank'    AND (s.Email IS NOT NULL AND s.Email <> ''))
                    OR (@EmailOperator2 = 'equals'      AND s.Email =          @EmailValue2)
                    OR (@EmailOperator2 = 'notEqual'    AND s.Email <>       @EmailValue2)
                    OR (@EmailOperator2 = 'contains'    AND s.Email LIKE '%' + @EmailValue2 + '%')
                    OR (@EmailOperator2 = 'notContains' AND s.Email NOT LIKE '%' + @EmailValue2 + '%')
                    OR (@EmailOperator2 = 'startsWith'  AND s.Email LIKE     @EmailValue2 + '%')
                    OR (@EmailOperator2 = 'endsWith'    AND s.Email LIKE '%' + @EmailValue2)
                    OR (@EmailOperator2 = 'blank'       AND (s.Email IS NULL OR s.Email = ''))
                    OR (@EmailOperator2 = 'notBlank'    AND (s.Email IS NOT NULL AND s.Email <> ''))
                )
            )
            OR 
            (
                @EmailFilterConcatOperator = 'AND'
                AND 
                (
                    (@EmailOperator1 = 'equals'      AND s.Email =          @EmailValue1)
                    OR (@EmailOperator1 = 'notEqual'    AND s.Email <>       @EmailValue1)
                    OR (@EmailOperator1 = 'contains'    AND s.Email LIKE '%' + @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'notContains' AND s.Email NOT LIKE '%' + @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'startsWith'  AND s.Email LIKE     @EmailValue1 + '%')
                    OR (@EmailOperator1 = 'endsWith'    AND s.Email LIKE '%' + @EmailValue1)
                    OR (@EmailOperator1 = 'blank'       AND (s.Email IS NULL OR s.Email = ''))
                    OR (@EmailOperator1 = 'notBlank'    AND (s.Email IS NOT NULL AND s.Email <> ''))
                )
                AND 
                (
                    (@EmailOperator2 = 'equals'      AND s.Email =          @EmailValue2)
                    OR (@EmailOperator2 = 'notEqual'    AND s.Email <>       @EmailValue2)
                    OR (@EmailOperator2 = 'contains'    AND s.Email LIKE '%' + @EmailValue2 + '%')
                    OR (@EmailOperator2 = 'notContains' AND s.Email NOT LIKE '%' + @EmailValue2 + '%')
                    OR (@EmailOperator2 = 'startsWith'  AND s.Email LIKE     @EmailValue2 + '%')
                    OR (@EmailOperator2 = 'endsWith'    AND s.Email LIKE '%' + @EmailValue2)
                    OR (@EmailOperator2 = 'blank'       AND (s.Email IS NULL OR s.Email = ''))
                    OR (@EmailOperator2 = 'notBlank'    AND (s.Email IS NOT NULL AND s.Email <> ''))
                )
            )
        )
        -- PhoneNumber filter
        AND 
        (
            @PhoneNumberOperator1 IS NULL
            OR 
            (
                @PhoneNumberOperator2 IS NULL
                AND 
                (
                    (@PhoneNumberOperator1 = 'equals'      AND s.PhoneNumber =          @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'notEqual'    AND s.PhoneNumber <>       @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'contains'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'notContains' AND s.PhoneNumber NOT LIKE '%' + @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'startsWith'  AND s.PhoneNumber LIKE     @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'endsWith'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'blank'       AND (s.PhoneNumber IS NULL OR s.PhoneNumber = ''))
                    OR (@PhoneNumberOperator1 = 'notBlank'    AND (s.PhoneNumber IS NOT NULL AND s.PhoneNumber <> ''))
                )
            )
            OR (
                @PhoneNumberFilterConcatOperator = 'OR'
                AND 
                (
                    (@PhoneNumberOperator1 = 'equals'      AND s.PhoneNumber =          @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'notEqual'    AND s.PhoneNumber <>       @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'contains'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'notContains' AND s.PhoneNumber NOT LIKE '%' + @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'startsWith'  AND s.PhoneNumber LIKE     @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'endsWith'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'blank'       AND (s.PhoneNumber IS NULL OR s.PhoneNumber = ''))
                    OR (@PhoneNumberOperator1 = 'notBlank'    AND (s.PhoneNumber IS NOT NULL AND s.PhoneNumber <> ''))
                    OR (@PhoneNumberOperator2 = 'equals'      AND s.PhoneNumber =          @PhoneNumberValue2)
                    OR (@PhoneNumberOperator2 = 'notEqual'    AND s.PhoneNumber <>       @PhoneNumberValue2)
                    OR (@PhoneNumberOperator2 = 'contains'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue2 + '%')
                    OR (@PhoneNumberOperator2 = 'notContains' AND s.PhoneNumber NOT LIKE '%' + @PhoneNumberValue2 + '%')
                    OR (@PhoneNumberOperator2 = 'startsWith'  AND s.PhoneNumber LIKE     @PhoneNumberValue2 + '%')
                    OR (@PhoneNumberOperator2 = 'endsWith'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue2)
                    OR (@PhoneNumberOperator2 = 'blank'       AND (s.PhoneNumber IS NULL OR s.PhoneNumber = ''))
                    OR (@PhoneNumberOperator2 = 'notBlank'    AND (s.PhoneNumber IS NOT NULL AND s.PhoneNumber <> ''))
                )
            )
            OR 
            (
                @PhoneNumberFilterConcatOperator = 'AND'
                AND 
                (
                    (@PhoneNumberOperator1 = 'equals'      AND s.PhoneNumber =          @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'notEqual'    AND s.PhoneNumber <>       @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'contains'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'notContains' AND s.PhoneNumber NOT LIKE '%' + @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'startsWith'  AND s.PhoneNumber LIKE     @PhoneNumberValue1 + '%')
                    OR (@PhoneNumberOperator1 = 'endsWith'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue1)
                    OR (@PhoneNumberOperator1 = 'blank'       AND (s.PhoneNumber IS NULL OR s.PhoneNumber = ''))
                    OR (@PhoneNumberOperator1 = 'notBlank'    AND (s.PhoneNumber IS NOT NULL AND s.PhoneNumber <> ''))
                )
                AND 
                (
                    (@PhoneNumberOperator2 = 'equals'      AND s.PhoneNumber =          @PhoneNumberValue2)
                    OR (@PhoneNumberOperator2 = 'notEqual'    AND s.PhoneNumber <>       @PhoneNumberValue2)
                    OR (@PhoneNumberOperator2 = 'contains'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue2 + '%')
                    OR (@PhoneNumberOperator2 = 'notContains' AND s.PhoneNumber NOT LIKE '%' + @PhoneNumberValue2 + '%')
                    OR (@PhoneNumberOperator2 = 'startsWith'  AND s.PhoneNumber LIKE     @PhoneNumberValue2 + '%')
                    OR (@PhoneNumberOperator2 = 'endsWith'    AND s.PhoneNumber LIKE '%' + @PhoneNumberValue2)
                    OR (@PhoneNumberOperator2 = 'blank'       AND (s.PhoneNumber IS NULL OR s.PhoneNumber = ''))
                    OR (@PhoneNumberOperator2 = 'notBlank'    AND (s.PhoneNumber IS NOT NULL AND s.PhoneNumber <> ''))
                )
            )
        )
        -- SSN filter
        AND 
        (
            @SSNOperator1 IS NULL
            OR 
            (
                @SSNOperator2 IS NULL
                AND (
                    (@SSNOperator1 = 'equals'      AND s.SSN =          @SSNValue1)
                    OR (@SSNOperator1 = 'notEqual'    AND s.SSN <>       @SSNValue1)
                    OR (@SSNOperator1 = 'contains'    AND s.SSN LIKE '%' + @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'notContains' AND s.SSN NOT LIKE '%' + @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'startsWith'  AND s.SSN LIKE     @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'endsWith'    AND s.SSN LIKE '%' + @SSNValue1)
                    OR (@SSNOperator1 = 'blank'       AND (s.SSN IS NULL OR s.SSN = ''))
                    OR (@SSNOperator1 = 'notBlank'    AND (s.SSN IS NOT NULL AND s.SSN <> ''))
                )
            )
            OR 
            (
                @SSNFilterConcatOperator = 'OR'
                AND 
                (
                    (@SSNOperator1 = 'equals'      AND s.SSN =          @SSNValue1)
                    OR (@SSNOperator1 = 'notEqual'    AND s.SSN <>       @SSNValue1)
                    OR (@SSNOperator1 = 'contains'    AND s.SSN LIKE '%' + @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'notContains' AND s.SSN NOT LIKE '%' + @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'startsWith'  AND s.SSN LIKE     @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'endsWith'    AND s.SSN LIKE '%' + @SSNValue1)
                    OR (@SSNOperator1 = 'blank'       AND (s.SSN IS NULL OR s.SSN = ''))
                    OR (@SSNOperator1 = 'notBlank'    AND (s.SSN IS NOT NULL AND s.SSN <> ''))
                    OR (@SSNOperator2 = 'equals'      AND s.SSN =          @SSNValue2)
                    OR (@SSNOperator2 = 'notEqual'    AND s.SSN <>       @SSNValue2)
                    OR (@SSNOperator2 = 'contains'    AND s.SSN LIKE '%' + @SSNValue2 + '%')
                    OR (@SSNOperator2 = 'notContains' AND s.SSN NOT LIKE '%' + @SSNValue2 + '%')
                    OR (@SSNOperator2 = 'startsWith'  AND s.SSN LIKE     @SSNValue2 + '%')
                    OR (@SSNOperator2 = 'endsWith'    AND s.SSN LIKE '%' + @SSNValue2)
                    OR (@SSNOperator2 = 'blank'       AND (s.SSN IS NULL OR s.SSN = ''))
                    OR (@SSNOperator2 = 'notBlank'    AND (s.SSN IS NOT NULL AND s.SSN <> ''))
                )
            )
            OR 
            (
                @SSNFilterConcatOperator = 'AND'
                AND 
                (
                    (@SSNOperator1 = 'equals'      AND s.SSN =          @SSNValue1)
                    OR (@SSNOperator1 = 'notEqual'    AND s.SSN <>       @SSNValue1)
                    OR (@SSNOperator1 = 'contains'    AND s.SSN LIKE '%' + @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'notContains' AND s.SSN NOT LIKE '%' + @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'startsWith'  AND s.SSN LIKE     @SSNValue1 + '%')
                    OR (@SSNOperator1 = 'endsWith'    AND s.SSN LIKE '%' + @SSNValue1)
                    OR (@SSNOperator1 = 'blank'       AND (s.SSN IS NULL OR s.SSN = ''))
                    OR (@SSNOperator1 = 'notBlank'    AND (s.SSN IS NOT NULL AND s.SSN <> ''))
                )
                AND 
                (
                    (@SSNOperator2 = 'equals'      AND s.SSN =          @SSNValue2)
                    OR (@SSNOperator2 = 'notEqual'    AND s.SSN <>       @SSNValue2)
                    OR (@SSNOperator2 = 'contains'    AND s.SSN LIKE '%' + @SSNValue2 + '%')
                    OR (@SSNOperator2 = 'notContains' AND s.SSN NOT LIKE '%' + @SSNValue2 + '%')
                    OR (@SSNOperator2 = 'startsWith'  AND s.SSN LIKE     @SSNValue2 + '%')
                    OR (@SSNOperator2 = 'endsWith'    AND s.SSN LIKE '%' + @SSNValue2)
                    OR (@SSNOperator2 = 'blank'       AND (s.SSN IS NULL OR s.SSN = ''))
                    OR (@SSNOperator2 = 'notBlank'    AND (s.SSN IS NOT NULL AND s.SSN <> ''))
                )
            )
        )
        -- StartDate filter
        AND
        (
            @StartDateOperator1 IS NULL
            OR
            (
                @StartDateOperator2 IS NULL
                AND
                (
                    (@StartDateOperator1 = 'equals'              AND CAST(s.StartDate AS DATE) =  CAST(@StartDateValue1 AS DATE))
                    OR (@StartDateOperator1 = 'notEqual'         AND CAST(s.StartDate AS DATE) <> CAST(@StartDateValue1 AS DATE))
                    OR (@StartDateOperator1 = 'greaterThan'      AND s.StartDate >  CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'greaterThanOrEqual' AND s.StartDate >= CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'lessThan'         AND s.StartDate <  CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'lessThanOrEqual'  AND s.StartDate <= CAST(@StartDateValue1 AS DATETIME))
                    OR 
                    (
                        @StartDateOperator1 = 'inRange'
                        AND s.StartDate >= CAST(@StartDateValue1 AS DATETIME) 
                        AND s.StartDate <= CAST(@StartDateValue2 AS DATETIME)
                    )
                    OR (@StartDateOperator1 = 'blank'            AND s.StartDate IS NULL)
                    OR (@StartDateOperator1 = 'notBlank'         AND s.StartDate IS NOT NULL)
                )
            )
            OR
            (
                @StartDateFilterConcatOperator = 'OR'
                AND
                (
                    (@StartDateOperator1 = 'equals'              AND CAST(s.StartDate AS DATE) =  CAST(@StartDateValue1 AS DATE))
                    OR (@StartDateOperator1 = 'notEqual'         AND CAST(s.StartDate AS DATE) <> CAST(@StartDateValue1 AS DATE))
                    OR (@StartDateOperator1 = 'greaterThan'      AND s.StartDate >  CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'greaterThanOrEqual' AND s.StartDate >= CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'lessThan'         AND s.StartDate <  CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'lessThanOrEqual'  AND s.StartDate <= CAST(@StartDateValue1 AS DATETIME))
                    OR 
                    (
                        @StartDateOperator1 = 'inRange'
                        AND s.StartDate >= CAST(@StartDateValue1 AS DATETIME) 
                        AND s.StartDate <= CAST(@StartDateValue2 AS DATETIME)
                    )
                    OR (@StartDateOperator1 = 'blank'            AND s.StartDate IS NULL)
                    OR (@StartDateOperator1 = 'notBlank'         AND s.StartDate IS NOT NULL)
                    OR (@StartDateOperator2 = 'equals'           AND CAST(s.StartDate AS DATE) =  CAST(@StartDateValue3 AS DATE))
                    OR (@StartDateOperator2 = 'notEqual'         AND CAST(s.StartDate AS DATE) <> CAST(@StartDateValue3 AS DATE))
                    OR (@StartDateOperator2 = 'greaterThan'      AND s.StartDate >  CAST(@StartDateValue3 AS DATETIME))
                    OR (@StartDateOperator2 = 'greaterThanOrEqual' AND s.StartDate >= CAST(@StartDateValue3 AS DATETIME))
                    OR (@StartDateOperator2 = 'lessThan'         AND s.StartDate <  CAST(@StartDateValue3 AS DATETIME))
                    OR (@StartDateOperator2 = 'lessThanOrEqual'  AND s.StartDate <= CAST(@StartDateValue3 AS DATETIME))
                    OR 
                    (
                        @StartDateOperator2 = 'inRange'
                        AND s.StartDate >= CAST(@StartDateValue3 AS DATETIME) 
                        AND s.StartDate <= CAST(@StartDateValue4 AS DATETIME)
                    )
                    OR (@StartDateOperator2 = 'blank'            AND s.StartDate IS NULL)
                    OR (@StartDateOperator2 = 'notBlank'         AND s.StartDate IS NOT NULL)
                )
            )
            OR
            (
                @StartDateFilterConcatOperator = 'AND'
                AND
                (
                    (@StartDateOperator1 = 'equals'              AND CAST(s.StartDate AS DATE) =  CAST(@StartDateValue1 AS DATE))
                    OR (@StartDateOperator1 = 'notEqual'         AND CAST(s.StartDate AS DATE) <> CAST(@StartDateValue1 AS DATE))
                    OR (@StartDateOperator1 = 'greaterThan'      AND s.StartDate >  CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'greaterThanOrEqual' AND s.StartDate >= CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'lessThan'         AND s.StartDate <  CAST(@StartDateValue1 AS DATETIME))
                    OR (@StartDateOperator1 = 'lessThanOrEqual'  AND s.StartDate <= CAST(@StartDateValue1 AS DATETIME))
                    OR 
                    (
                        @StartDateOperator1 = 'inRange'
                        AND s.StartDate >= CAST(@StartDateValue1 AS DATETIME) 
                        AND s.StartDate <= CAST(@StartDateValue2 AS DATETIME)
                    )
                    OR (@StartDateOperator1 = 'blank'            AND s.StartDate IS NULL)
                    OR (@StartDateOperator1 = 'notBlank'         AND s.StartDate IS NOT NULL)
                )
                AND
                (
                    (@StartDateOperator2 = 'equals'              AND CAST(s.StartDate AS DATE) =  CAST(@StartDateValue3 AS DATE))
                    OR (@StartDateOperator2 = 'notEqual'         AND CAST(s.StartDate AS DATE) <> CAST(@StartDateValue3 AS DATE))
                    OR (@StartDateOperator2 = 'greaterThan'      AND s.StartDate >  CAST(@StartDateValue3 AS DATETIME))
                    OR (@StartDateOperator2 = 'greaterThanOrEqual' AND s.StartDate >= CAST(@StartDateValue3 AS DATETIME))
                    OR (@StartDateOperator2 = 'lessThan'         AND s.StartDate <  CAST(@StartDateValue3 AS DATETIME))
                    OR (@StartDateOperator2 = 'lessThanOrEqual'  AND s.StartDate <= CAST(@StartDateValue3 AS DATETIME))
                    OR 
                    (
                        @StartDateOperator2 = 'inRange'
                        AND s.StartDate >= CAST(@StartDateValue3 AS DATETIME) 
                        AND s.StartDate <= CAST(@StartDateValue4 AS DATETIME)
                    )
                    OR (@StartDateOperator2 = 'blank'            AND s.StartDate IS NULL)
                    OR (@StartDateOperator2 = 'notBlank'         AND s.StartDate IS NOT NULL)
                )
            )
        )
        -- IsActive filter
        AND (@IsActive IS NULL OR s.IsActive = @IsActive)
    ORDER BY
        -- firstName
        CASE WHEN @SortBy = 'firstName'   AND LOWER(@SortByOperator) != 'desc' THEN s.FirstName   END ASC,
        CASE WHEN @SortBy = 'firstName'   AND LOWER(@SortByOperator) =  'desc' THEN s.FirstName   END DESC,
        -- lastName
        CASE WHEN @SortBy = 'lastName'    AND LOWER(@SortByOperator) != 'desc' THEN s.LastName     END ASC,
        CASE WHEN @SortBy = 'lastName'    AND LOWER(@SortByOperator) =  'desc' THEN s.LastName     END DESC,
        -- email
        CASE WHEN @SortBy = 'email'       AND LOWER(@SortByOperator) != 'desc' THEN s.Email        END ASC,
        CASE WHEN @SortBy = 'email'       AND LOWER(@SortByOperator) =  'desc' THEN s.Email        END DESC,
        -- phoneNumber
        CASE WHEN @SortBy = 'phoneNumber' AND LOWER(@SortByOperator) != 'desc' THEN s.PhoneNumber  END ASC,
        CASE WHEN @SortBy = 'phoneNumber' AND LOWER(@SortByOperator) =  'desc' THEN s.PhoneNumber  END DESC,
        -- ssn
        CASE WHEN @SortBy = 'ssn'         AND LOWER(@SortByOperator) != 'desc' THEN s.SSN          END ASC,
        CASE WHEN @SortBy = 'ssn'         AND LOWER(@SortByOperator) =  'desc' THEN s.SSN          END DESC,
        -- startDate
        CASE WHEN @SortBy = 'startDate'   AND LOWER(@SortByOperator) != 'desc' THEN s.StartDate    END ASC,
        CASE WHEN @SortBy = 'startDate'   AND LOWER(@SortByOperator) =  'desc' THEN s.StartDate    END DESC,
        -- isActive (BIT: 0 = false, 1 = true)
        CASE WHEN @SortBy = 'isActive'    AND LOWER(@SortByOperator) != 'desc' THEN s.IsActive     END ASC,
        CASE WHEN @SortBy = 'isActive'    AND LOWER(@SortByOperator) =  'desc' THEN s.IsActive     END DESC,
        -- createdDate (explicit sort)
        CASE WHEN @SortBy = 'createdDate' AND LOWER(@SortByOperator) != 'desc' THEN s.CreatedDate  END ASC,
        CASE WHEN @SortBy = 'createdDate' AND LOWER(@SortByOperator) =  'desc' THEN s.CreatedDate  END DESC,
        -- default: CreatedDate ASC when no sort column is specified
        CASE WHEN @SortBy IS NULL THEN s.CreatedDate END ASC,
        -- secondary (tiebreaker): always Id DESC
        s.Id DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY
    OPTION (RECOMPILE);
END