/**
 * Global error codes for the application
 * These codes should match the C# SCHErrorNumber enum in SCH.Shared.Exceptions
 */
export enum SCHErrorNumber {
  /**
   * General/unknown error
   */
  None = 0,

  /**
   * Concurrency conflict - record was modified by another user
   */
  ConcurrencyConflict = 1
}
