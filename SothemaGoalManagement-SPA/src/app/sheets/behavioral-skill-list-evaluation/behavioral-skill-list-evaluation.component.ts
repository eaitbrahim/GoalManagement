import { Component, OnInit, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { BehavioralSkillInstance } from '../../_models/behavioralSkillInstance';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-behavioral-skill-list-evaluation',
  templateUrl: './behavioral-skill-list-evaluation.component.html',
  styleUrls: ['./behavioral-skill-list-evaluation.component.css']
})
export class BehavioralSkillListEvaluationComponent implements OnInit {
  @Input() behavioralSkillInstanceList: BehavioralSkillInstance[];
  @Input() sheetId: number;
  @Input() sheetOwnerId: number;
  @Input() areBehavioralSkillsEvaluable: boolean;
  @Output() addBehavioralSkillEvaluationEvent = new EventEmitter<any[]>();
  @Output() behavioralSkillEvaluationUpdatedEvent = new EventEmitter<boolean>();
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.dirty) {
      $event.returnValue = true;
    }
  }
  displayDefinition: boolean;
  description: string;
  evals: any[] = [];
  faCheckCircle = faCheckCircle;
  dirty: boolean;
  totalGrade: string = "0.00";

  constructor(private alertify: AlertifyService) { }

  ngOnInit() {
    this.evals = this.behavioralSkillInstanceList.map(behavioralSkillInstance => ({
      grade: behavioralSkillInstance.behavioralSkillGrade.toFixed(2),
      level: behavioralSkillInstance.behavioralSkillLevel,
      behavioralSkillInstanceId: behavioralSkillInstance.id,
      evaluateeId: this.sheetOwnerId,
      evaluationFileInstanceId: this.sheetId
    }));

    this.calculateTotalGrade();
  }

  changeEventInRadioButton(behavioralSkillInstance: BehavioralSkillInstance, newGrade: number) {
    let newEval = {
      grade: newGrade,
      level: this.getLevel(behavioralSkillInstance, newGrade),
      behavioralSkillInstanceId: behavioralSkillInstance.id,
      evaluateeId: this.sheetOwnerId,
      evaluationFileInstanceId: this.sheetId
    };

    this.evals.splice(this.evals.findIndex(e => e.behavioralSkillInstanceId === behavioralSkillInstance.id), 1, newEval);
    this.calculateTotalGrade();

    this.dirty = true;
    this.behavioralSkillEvaluationUpdatedEvent.emit(true);
  }

  calculateTotalGrade() {
    let totalGrade = 0;
    for (let e of this.evals) {
      totalGrade += Number(e.grade);
    }
    this.totalGrade = (totalGrade / this.evals.length).toFixed(2);
  }

  enableSave() {
    return this.evals.filter(e => e.level === "").length > 0 ? true : false;
  }

  getLevel(behavioralSkillInstance: BehavioralSkillInstance, grade: number) {
    if (grade == behavioralSkillInstance.levelOneGrade) return behavioralSkillInstance.levelOne;
    if (grade == behavioralSkillInstance.levelTwoGrade) return behavioralSkillInstance.levelTwo;
    if (grade == behavioralSkillInstance.levelThreeGrade) return behavioralSkillInstance.levelThree;
    if (grade == behavioralSkillInstance.levelFourGrade) return behavioralSkillInstance.levelFour;

    return '';
  }

  save() {
    this.alertify.confirm('Confirmer',
      `Êtes-vous sûr de vouloir ajouter cette évaluation:
        <ul>
        ${this.evals.map(e => "<li>" + this.behavioralSkillInstanceList.find(b => b.id === e.behavioralSkillInstanceId).skill + ": " + e.level + "</li>").join("")}
        </ul>
        `,
      () => {
        this.addBehavioralSkillEvaluationEvent.emit(this.evals);
        this.dirty = false;
        this.behavioralSkillEvaluationUpdatedEvent.emit(false);
      }
    );
  }

  levelSelected(description) {
    this.description = description;
    this.displayDefinition = true;
  }
}
