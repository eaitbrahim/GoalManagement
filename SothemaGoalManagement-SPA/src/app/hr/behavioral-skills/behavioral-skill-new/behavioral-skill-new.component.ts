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
import { BehavioralSkill } from '../../../_models/behavioralSkill';


@Component({
  selector: 'app-behavioral-skill-new',
  templateUrl: './behavioral-skill-new.component.html',
  styleUrls: ['./behavioral-skill-new.component.css']
})
export class BehavioralSkillNewComponent implements OnInit {
  @Output() cancelCreation = new EventEmitter();
  @Output() switchOffCreation = new EventEmitter();
  newBehavioralSkill: BehavioralSkill;
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
        skill: ['', Validators.required],
        definition: ['', [Validators.required]],
        levelOne: ['Insatisfaisant', Validators.required],
        levelOneGrade: ['1', [Validators.required]],
        levelOneDescription: [''],
        levelTwo: ['A améliorer', Validators.required],
        levelTwoGrade: ['2', [Validators.required]],
        levelTwoDescription: [''],
        levelThree: ['Conforme aux attentes', Validators.required],
        levelThreeGrade: ['3', [Validators.required]],
        levelThreeDescription: [''],
        levelFour: ['Supérieur aux attentes', Validators.required],
        levelFourGrade: ['4', [Validators.required]],
        levelFourDescription: ['']
      });
  }

  create() {
    if (this.newForm.valid) {
      this.newBehavioralSkill = Object.assign({}, this.newForm.value);
      this.loading = true;
      this.hrService.createBehavioralSkill(this.authService.decodedToken.nameid, this.newBehavioralSkill).subscribe(
        () => {
          this.loading = false;
          this.alertify.success('Compétence comportementale créé avec succèes');
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
