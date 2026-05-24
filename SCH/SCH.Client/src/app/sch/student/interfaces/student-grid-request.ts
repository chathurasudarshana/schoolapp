import { GridRequest } from '../../../interfaces/grid-request';

export interface StudentGridRequest extends GridRequest {
  firstName?: string | null;
  firstNameOperator?: string | null;

  lastName?: string | null;
  lastNameOperator?: string | null;

  email?: string | null;
  emailOperator?: string | null;

  phoneNumber?: string | null;
  phoneNumberOperator?: string | null;

  ssn?: string | null;
  ssnOperator?: string | null;

  /** ISO date string, e.g. '2024-01-15' */
  startDate?: string | null;
  startDateOperator?: string | null;

  isActive?: boolean | null;
}
