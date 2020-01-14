import { filter } from 'rxjs/operators';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faCaretDown, faCaretUp, faPlus } from '@fortawesome/free-solid-svg-icons';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';

import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';
import { Goal } from '../../_models/goal';
import { GoalEvaluationModalComponent } from '../goal-evaluation-modal/goal-evaluation-modal.component';

@Component({
  selector: 'app-auto-evaluation-axis',
  templateUrl: './auto-evaluation-axis.component.html',
  styleUrls: ['./auto-evaluation-axis.component.css']
})
export class AutoEvaluationAxisComponent implements OnInit {
  @Input() goalsByAxisInstance: GoalByAxisInstance;
  @Input() sheetOwnerId: number;
  @Input() sheetStatus: string;
  @Output() addGoalEvaluationEvent = new EventEmitter<any>();
  isCollapsed: boolean;
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;
  faPlus = faPlus;
  bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  ngOnInit() {
    this.isCollapsed = false;
  }

  handleAddGoalEvaluation(newEval: any) {
    this.addGoalEvaluationEvent.emit(newEval);
  }

  addEvaluation(goal) {
    const initialState = {
      goal: goal,
      evaluateeId: this.sheetOwnerId,
      sheetStatus: this.sheetStatus
    };

    this.bsModalRef = this.modalService.show(GoalEvaluationModalComponent, { initialState, ignoreBackdropClick: true, class: 'modal-lg' });
    this.bsModalRef.content.addGoalEvaluationEvent.subscribe((newEval) => {
      this.addGoalEvaluationEvent.emit(newEval);
    });
  }
}
