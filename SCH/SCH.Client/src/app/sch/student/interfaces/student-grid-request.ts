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

  startDateOperator1?: string | null;
  startDateValue1?: string | null;
  startDateValue2?: string | null;
  startDateFilterConcatOperator?: string | null;
  startDateOperator2?: string | null;
  startDateValue3?: string | null;
  startDateValue4?: string | null;

  isActive?: boolean | null;
}
