import { filter } from 'rxjs/operators';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {
  FormGroup,
  FormArray,
  FormControl,
  ValidatorFn,
  Validators,
  FormBuilder,
  AbstractControl
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { of } from 'rxjs';

import { HrService } from '../../../_services/hr.service';
import { AuthService } from '../../../_services/auth.service';
import { EvaluationFile } from '../../../_models/evaluationFile';
import { Strategy } from '../../../_models/strategy';
import { BehavioralSkill } from '../../../_models/behavioralSkill';
import { AlertifyService } from '../../../_services/alertify.service';

@Component({
  selector: 'app-evaluation-hr-new',
  templateUrl: './evaluation-hr-new.component.html',
  styleUrls: ['./evaluation-hr-new.component.css']
})
export class EvaluationHrNewComponent implements OnInit {
  @Output() cancelCreation = new EventEmitter();
  @Output() switchOffCreation = new EventEmitter();
  newForm: FormGroup;
  public loading = false;
  strategyList: Strategy[];
  skillsData: BehavioralSkill[];

  constructor(private fb: FormBuilder, private hrService: HrService, private authService: AuthService, private alertify: AlertifyService) {
  }

  ngOnInit() {
    this.loadPublishedStratgeies();
    this.loadPublishedBehavioralSkills();
    this.createForm();
  }

  loadPublishedStratgeies() {
    this.loading = true;
    this.hrService.getPublishedStrategies().subscribe(
      (result: Strategy[]) => {
        this.loading = false;
        this.strategyList = result.filter(r => r.sealed === false);
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  loadPublishedBehavioralSkills() {
    this.loading = true;
    of(this.hrService.getPublishedBehavioralSkills().subscribe(
      (result: BehavioralSkill[]) => {
        this.loading = false;
        this.skillsData = result; // .filter(r => r.sealed === false);;
        this.addCheckboxes();
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    ));
  }

  createForm() {
    this.newForm = this.fb.group(
      {
        title: ['', Validators.required],
        year: ['', Validators.required],
        strategy: ['', Validators.required],
        skills: new FormArray([], this.minSelectedCheckboxes(1))
      });
  }

  create() {
    if (this.newForm.valid) {
      const selectedSkillIds = this.newForm.value.skills.map((v, i) => v ? this.skillsData[i].id : null).filter(v => v !== null);
      const newEvaluationFile = {
        title: this.newForm.value.title,
        year: this.newForm.value.year,
        strategyId: this.newForm.value.strategy.id,
        behavioralSkillIds: selectedSkillIds
      };
      this.loading = true;
      this.hrService.createEvaluationFile(this.authService.decodedToken.nameid, newEvaluationFile).subscribe(
        () => {
          this.loading = false;
          this.alertify.success('Fiche d\'évaluation créé avec succèes');
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

  private addCheckboxes() {
    this.skillsData.forEach((o, i) => {
      const control = new FormControl(true);
      (this.newForm.get('skills') as FormArray).push(control);
    });
  }

  minSelectedCheckboxes(min = 1) {
    const validator: ValidatorFn = (formArray: FormArray) => {
      const totalSelected = formArray.controls
        // get a list of checkbox values (boolean)
        .map(control => control.value)
        // total up the number of checked checkboxes
        .reduce((prev, next) => next ? prev + next : prev, 0);

      // if the total is not greater than the minimum, return the error message
      return totalSelected >= min ? null : { required: true };
    };

    return validator;
  }
}
