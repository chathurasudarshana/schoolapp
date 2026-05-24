CREATE PROCEDURE [dbo].[GetStudentGrid]
    @PageNumber          INT,
    @PageSize            INT,
    @SortBy              NVARCHAR(100)  = NULL,
    @SortByOperator      NVARCHAR(10)   = NULL,   -- 'asc' | 'desc'
    @FirstName           NVARCHAR(400)  = NULL,
    @FirstNameOperator   NVARCHAR(20)   = NULL,   -- 'eq' | 'ne' | 'contains' | 'startswith' | 'endswith'
    @LastName            NVARCHAR(400)  = NULL,
    @LastNameOperator    NVARCHAR(20)   = NULL,
    @Email               NVARCHAR(400)  = NULL,
    @EmailOperator       NVARCHAR(20)   = NULL,
    @PhoneNumber         NVARCHAR(20)   = NULL,
    @PhoneNumberOperator NVARCHAR(20)   = NULL,
    @SSN                 NVARCHAR(20)   = NULL,
    @SSNOperator         NVARCHAR(20)   = NULL,
    @StartDate           NVARCHAR(30)   = NULL,   -- ISO date string, e.g. '2024-01-15'
    @StartDateOperator   NVARCHAR(20)   = NULL,   -- 'eq' | 'ne' | 'gt' | 'gte' | 'lt' | 'lte'
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
            @FirstNameOperator IS NULL
            OR (@FirstNameOperator = 'equals'      AND s.FirstName =      @FirstName)
            OR (@FirstNameOperator = 'notEqual'    AND s.FirstName <>     @FirstName)
            OR (@FirstNameOperator = 'contains'    AND s.FirstName LIKE '%' + @FirstName + '%')
            OR (@FirstNameOperator = 'notContains' AND s.FirstName NOT LIKE '%' + @FirstName + '%')
            OR (@FirstNameOperator = 'startsWith'  AND s.FirstName LIKE   @FirstName + '%')
            OR (@FirstNameOperator = 'endsWith'    AND s.FirstName LIKE '%' + @FirstName)
            OR (@FirstNameOperator = 'blank'       AND (s.FirstName IS NULL OR s.FirstName = ''))
            OR (@FirstNameOperator = 'notBlank'    AND (s.FirstName IS NOT NULL AND s.FirstName <> ''))
        )
        -- LastName filter
        AND (
            @LastNameOperator IS NULL
            OR (@LastNameOperator = 'equals'      AND s.LastName =      @LastName)
            OR (@LastNameOperator = 'notEqual'    AND s.LastName <>     @LastName)
            OR (@LastNameOperator = 'contains'    AND s.LastName LIKE '%' + @LastName + '%')
            OR (@LastNameOperator = 'notContains' AND s.LastName NOT LIKE '%' + @LastName + '%')
            OR (@LastNameOperator = 'startsWith'  AND s.LastName LIKE   @LastName + '%')
            OR (@LastNameOperator = 'endsWith'    AND s.LastName LIKE '%' + @LastName)
            OR (@LastNameOperator = 'blank'       AND (s.LastName IS NULL OR s.LastName = ''))
            OR (@LastNameOperator = 'notBlank'    AND (s.LastName IS NOT NULL AND s.LastName <> ''))
        )
        -- Email filter
        AND (
            @EmailOperator IS NULL
            OR (@EmailOperator = 'equals'      AND s.Email =      @Email)
            OR (@EmailOperator = 'notEqual'    AND s.Email <>     @Email)
            OR (@EmailOperator = 'contains'    AND s.Email LIKE '%' + @Email + '%')
            OR (@EmailOperator = 'notContains' AND s.Email NOT LIKE '%' + @Email + '%')
            OR (@EmailOperator = 'startsWith'  AND s.Email LIKE   @Email + '%')
            OR (@EmailOperator = 'endsWith'    AND s.Email LIKE '%' + @Email)
            OR (@EmailOperator = 'blank'       AND (s.Email IS NULL OR s.Email = ''))
            OR (@EmailOperator = 'notBlank'    AND (s.Email IS NOT NULL AND s.Email <> ''))
        )
        -- PhoneNumber filter
        AND (
            @PhoneNumberOperator IS NULL
            OR (@PhoneNumberOperator = 'equals'      AND s.PhoneNumber =      @PhoneNumber)
            OR (@PhoneNumberOperator = 'notEqual'    AND s.PhoneNumber <>     @PhoneNumber)
            OR (@PhoneNumberOperator = 'contains'    AND s.PhoneNumber LIKE '%' + @PhoneNumber + '%')
            OR (@PhoneNumberOperator = 'notContains' AND s.PhoneNumber NOT LIKE '%' + @PhoneNumber + '%')
            OR (@PhoneNumberOperator = 'startsWith'  AND s.PhoneNumber LIKE   @PhoneNumber + '%')
            OR (@PhoneNumberOperator = 'endsWith'    AND s.PhoneNumber LIKE '%' + @PhoneNumber)
            OR (@PhoneNumberOperator = 'blank'       AND (s.PhoneNumber IS NULL OR s.PhoneNumber = ''))
            OR (@PhoneNumberOperator = 'notBlank'    AND (s.PhoneNumber IS NOT NULL AND s.PhoneNumber <> ''))
        )
        -- SSN filter
        AND (
            @SSNOperator IS NULL
            OR (@SSNOperator = 'equals'      AND s.SSN =      @SSN)
            OR (@SSNOperator = 'notEqual'    AND s.SSN <>     @SSN)
            OR (@SSNOperator = 'contains'    AND s.SSN LIKE '%' + @SSN + '%')
            OR (@SSNOperator = 'notContains' AND s.SSN NOT LIKE '%' + @SSN + '%')
            OR (@SSNOperator = 'startsWith'  AND s.SSN LIKE   @SSN + '%')
            OR (@SSNOperator = 'endsWith'    AND s.SSN LIKE '%' + @SSN)
            OR (@SSNOperator = 'blank'       AND (s.SSN IS NULL OR s.SSN = ''))
            OR (@SSNOperator = 'notBlank'    AND (s.SSN IS NOT NULL AND s.SSN <> ''))
        )
        -- StartDate filter
        AND (
            @StartDate IS NULL
            OR (@StartDateOperator = 'eq'  AND CAST(s.StartDate AS DATE) =  CAST(@StartDate AS DATE))
            OR (@StartDateOperator = 'ne'  AND CAST(s.StartDate AS DATE) <> CAST(@StartDate AS DATE))
            OR (@StartDateOperator = 'gt'  AND s.StartDate >  CAST(@StartDate AS DATETIME))
            OR (@StartDateOperator = 'gte' AND s.StartDate >= CAST(@StartDate AS DATETIME))
            OR (@StartDateOperator = 'lt'  AND s.StartDate <  CAST(@StartDate AS DATETIME))
            OR (@StartDateOperator = 'lte' AND s.StartDate <= CAST(@StartDate AS DATETIME))
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