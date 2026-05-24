import { GridRequest } from '../../../interfaces/grid-request';

export interface StudentGridRequest extends GridRequest {
  firstNameValue1?: string | null;
  firstNameOperator1?: string | null;
  firstNameValue2?: string | null;
  firstNameOperator2?: string | null;
  firstNameFilterConcatOperator?: string | null;

  lastNameValue1?: string | null;
  lastNameOperator1?: string | null;
  lastNameValue2?: string | null;
  lastNameOperator2?: string | null;
  lastNameFilterConcatOperator?: string | null;

  emailValue1?: string | null;
  emailOperator1?: string | null;
  emailValue2?: string | null;
  emailOperator2?: string | null;
  emailFilterConcatOperator?: string | null;

  phoneNumberValue1?: string | null;
  phoneNumberOperator1?: string | null;
  phoneNumberValue2?: string | null;
  phoneNumberOperator2?: string | null;
  phoneNumberFilterConcatOperator?: string | null;

  ssnValue1?: string | null;
  ssnOperator1?: string | null;
  ssnValue2?: string | null;
  ssnOperator2?: string | null;
  ssnFilterConcatOperator?: string | null;

  /** ISO date string, e.g. '2024-01-15' */
  startDate?: string | null;
  startDateOperator?: string | null;

  isActive?: boolean | null;
}
