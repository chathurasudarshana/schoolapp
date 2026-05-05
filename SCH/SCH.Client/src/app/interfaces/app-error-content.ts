/**
 * Application error content interface
 * Mirrors the C# AppErrorContent class from SCH.Core.ErrorHandling
 */
export interface AppErrorContent {
  message: string;
  trace?: string | null;
  data?: { [key: string | number]: any };
}
