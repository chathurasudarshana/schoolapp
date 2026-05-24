CREATE   PROCEDURE [dbo].[GetStudentGrid]
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
            @FirstName IS NULL
            OR (@FirstNameOperator = 'eq'         AND s.FirstName =  @FirstName)
            OR (@FirstNameOperator = 'ne'         AND s.FirstName <> @FirstName)
            OR (@FirstNameOperator = 'contains'   AND s.FirstName LIKE '%' + @FirstName + '%')
            OR (@FirstNameOperator = 'startswith' AND s.FirstName LIKE @FirstName + '%')
            OR (@FirstNameOperator = 'endswith'   AND s.FirstName LIKE '%' + @FirstName)
        )
        -- LastName filter
        AND (
            @LastName IS NULL
            OR (@LastNameOperator = 'eq'         AND s.LastName =  @LastName)
            OR (@LastNameOperator = 'ne'         AND s.LastName <> @LastName)
            OR (@LastNameOperator = 'contains'   AND s.LastName LIKE '%' + @LastName + '%')
            OR (@LastNameOperator = 'startswith' AND s.LastName LIKE @LastName + '%')
            OR (@LastNameOperator = 'endswith'   AND s.LastName LIKE '%' + @LastName)
        )
        -- Email filter
        AND (
            @Email IS NULL
            OR (@EmailOperator = 'eq'         AND s.Email =  @Email)
            OR (@EmailOperator = 'ne'         AND s.Email <> @Email)
            OR (@EmailOperator = 'contains'   AND s.Email LIKE '%' + @Email + '%')
            OR (@EmailOperator = 'startswith' AND s.Email LIKE @Email + '%')
            OR (@EmailOperator = 'endswith'   AND s.Email LIKE '%' + @Email)
        )
        -- PhoneNumber filter
        AND (
            @PhoneNumber IS NULL
            OR (@PhoneNumberOperator = 'eq'         AND s.PhoneNumber =  @PhoneNumber)
            OR (@PhoneNumberOperator = 'ne'         AND s.PhoneNumber <> @PhoneNumber)
            OR (@PhoneNumberOperator = 'contains'   AND s.PhoneNumber LIKE '%' + @PhoneNumber + '%')
            OR (@PhoneNumberOperator = 'startswith' AND s.PhoneNumber LIKE @PhoneNumber + '%')
            OR (@PhoneNumberOperator = 'endswith'   AND s.PhoneNumber LIKE '%' + @PhoneNumber)
        )
        -- SSN filter
        AND (
            @SSN IS NULL
            OR (@SSNOperator = 'eq'         AND s.SSN =  @SSN)
            OR (@SSNOperator = 'ne'         AND s.SSN <> @SSN)
            OR (@SSNOperator = 'contains'   AND s.SSN LIKE '%' + @SSN + '%')
            OR (@SSNOperator = 'startswith' AND s.SSN LIKE @SSN + '%')
            OR (@SSNOperator = 'endswith'   AND s.SSN LIKE '%' + @SSN)
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