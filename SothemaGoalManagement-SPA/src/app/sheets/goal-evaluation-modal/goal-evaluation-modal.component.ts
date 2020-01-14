import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { Goal } from '../../_models/goal';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-goal-evaluation-modal',
  templateUrl: './goal-evaluation-modal.component.html',
  styleUrls: ['./goal-evaluation-modal.component.css']
})
export class GoalEvaluationModalComponent implements OnInit {
  @Output() addGoalEvaluationEvent = new EventEmitter<any>();
  goal: Goal;
  newEval: any;
  evaluateeId: number;
  sheetStatus: string;

  constructor(public bsModalRef: BsModalRef, private alertify: AlertifyService) { }

  ngOnInit() {
    this.newEval = { completionRate: 0, comment: '', goalId: this.goal.id, evaluateeId: this.evaluateeId };
  }

  addEvaluation() {
    this.alertify.confirm('Confirmer',
      `Êtes-vous sûr de vouloir ajouter cette évaluation avec un taux de réalisation: ${this.newEval.completionRate} %?`,
      () => {
        this.addGoalEvaluationEvent.emit(this.newEval);
        this.bsModalRef.hide();
      }
    );
  }

}
