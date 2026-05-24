export interface GridRequest {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string | null;
  /** 'asc' | 'desc' */
  sortByOperator?: string | null;
}
