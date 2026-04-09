import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class AppValidators {
  /**
   * Validates that a password meets complexity requirements:
   * 8 characters minimum, 1 uppercase, 1 lowercase, 1 number, 1 special character.
   */
  static passwordComplexity(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;

      const hasUpperCase = /[A-Z]/.test(value);
      const hasLowerCase = /[a-z]/.test(value);
      const hasNumeric = /[0-9]/.test(value);
      const hasSpecial = /[\W_]/.test(value);
      const isValidLength = value.length >= 8;

      const valid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecial && isValidLength;

      if (!valid) {
        return {
          passwordComplexity: {
            valid: false,
            requirements: 'Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.'
          }
        };
      }
      return null;
    };
  }

  /**
   * Validates that two fields match (e.g. password and confirmPassword).
   * Usage: formGroup.setValidators(AppValidators.matchFields('password', 'confirmPassword'))
   */
  static matchFields(controlName: string, matchingControlName: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const control = formGroup.get(controlName);
      const matchingControl = formGroup.get(matchingControlName);

      if (!control || !matchingControl) {
        return null;
      }

      // Return if another validator has already found an error on the matchingControl
      if (matchingControl.errors && !matchingControl.errors['mustMatch']) {
        return null;
      }

      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
        return { mustMatch: true };
      } else {
        matchingControl.setErrors(null);
        return null;
      }
    };
  }
}
