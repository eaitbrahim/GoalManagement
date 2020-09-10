import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
  AbstractControl
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';

import { AlertifyService } from '../../../_services/alertify.service';
import { HrService } from '../../../_services/hr.service';
import { AuthService } from '../../../_services/auth.service';
import { Strategy } from '../../../_models/strategy';

@Component({
  selector: 'app-strategy-new',
  templateUrl: './strategy-new.component.html',
  styleUrls: ['./strategy-new.component.css']
})
export class StrategyNewComponent implements OnInit {
  @Output() cancelCreation = new EventEmitter();
  @Output() switchOffCreation = new EventEmitter();
  newStrategy: Strategy;
  newForm: FormGroup;
  public loading = false;

  constructor(private fb: FormBuilder,
    private hrService: HrService,
    private authService: AuthService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.createForm();
  }

  createForm() {
    this.newForm = this.fb.group(
      {
        title: ['', Validators.required],
        description: ['', [Validators.required]]
      });
  }

  create() {
    if (this.newForm.valid) {
      this.newStrategy = Object.assign({}, this.newForm.value);
      this.loading = true;
      this.hrService.createStrategy(this.authService.decodedToken.nameid, this.newStrategy).subscribe(
        () => {
          this.loading = false;
          this.alertify.success('Stratégie créé avec succèes');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        },
        () => { this.switchOffCreation.emit(true); }
      );
    }
  }

  cancel() {
    this.cancelCreation.emit(false);
  }
}
