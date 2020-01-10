import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder
} from '@angular/forms';
import { faKey, faHandshake, faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

import { AlertifyService } from '../../_services/alertify.service';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  model: any = {};
  resetPasswordForm: FormGroup;
  public loading = false;
  faKey = faKey;
  faHandshake = faHandshake;
  faEye = faEye;
  faEyeSlash = faEyeSlash;
  minLength: number = 8;
  maxLength: number = 24;
  togglePassword: boolean;
  inputType: string = 'password';

  constructor(private fb: FormBuilder, private router: Router, private route: ActivatedRoute, private authService: AuthService,
    private alertify: AlertifyService, ) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.model.token = params['token'];
      this.model.email = params['email']
    });

    this.createResetPasswordForm();
  }

  createResetPasswordForm() {
    this.resetPasswordForm = this.fb.group(
      {
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(this.minLength),
            Validators.maxLength(this.maxLength)
          ]
        ],
        confirmPassword: ['', Validators.required],
        token: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  resetPassword() {
    this.loading = true;
    this.authService.resetPassword(this.model).subscribe(
      next => {
        this.loading = false;
        this.alertify.success('Réinitialisation avec succès');
      },
      error => {
        this.loading = false;
        this.alertify.error('Le mot de passe doit respecter les règles citées ci-dessus sinon votre token a expiré.');
      },
      () => {
        this.router.navigate(['/home']);
      }
    );
  }

  togglePasswordOnClick() {
    this.togglePassword = !this.togglePassword;
    if (this.togglePassword) {
      this.inputType = 'text';
    } else {
      this.inputType = 'password';
    }
  }
}
