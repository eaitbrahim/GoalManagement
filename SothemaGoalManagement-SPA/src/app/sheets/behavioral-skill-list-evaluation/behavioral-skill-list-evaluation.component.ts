import { Component, OnInit, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { faCheckCircle, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
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
  faTimesCircle = faTimesCircle;
  dirty: boolean;
  totalGrade: number;

  constructor(private alertify: AlertifyService) { }

  ngOnInit() {
    this.evals = this.behavioralSkillInstanceList.map(behavioralSkillInstance => ({
      grade: behavioralSkillInstance.behavioralSkillGrade,
      level: behavioralSkillInstance.behavioralSkillLevel,
      behavioralSkillInstanceId: behavioralSkillInstance.id,
      evaluateeId: this.sheetOwnerId,
      evaluationFileInstanceId: this.sheetId
    }));

    this.totalGrade = this.evals.reduce((acc, curr) => acc + curr.grade, 0);
  }

  changeEventInRadioButton(behavioralSkillInstance: BehavioralSkillInstance, newGrade: string) {
    let newEval = {
      grade: parseInt(newGrade),
      level: this.getLevel(behavioralSkillInstance, newGrade),
      behavioralSkillInstanceId: behavioralSkillInstance.id,
      evaluateeId: this.sheetOwnerId,
      evaluationFileInstanceId: this.sheetId
    };

    this.evals.splice(this.evals.findIndex(e => e.behavioralSkillInstanceId === behavioralSkillInstance.id), 1, newEval);
    this.totalGrade = this.evals.reduce((acc, curr) => acc + curr.grade, 0);
    this.dirty = true;
    this.behavioralSkillEvaluationUpdatedEvent.emit(true);
  }

  getLevel(behavioralSkillInstance: BehavioralSkillInstance, grade: string) {
    if (grade == behavioralSkillInstance.levelOneGrade.toString()) return behavioralSkillInstance.levelOne;
    if (grade == behavioralSkillInstance.levelTwoGrade.toString()) return behavioralSkillInstance.levelTwo;
    if (grade == behavioralSkillInstance.levelThreeGrade.toString()) return behavioralSkillInstance.levelThree;
    if (grade == behavioralSkillInstance.levelFourGrade.toString()) return behavioralSkillInstance.levelFour;

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
