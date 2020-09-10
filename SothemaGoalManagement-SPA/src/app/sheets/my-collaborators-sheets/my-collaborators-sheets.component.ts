import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { AxisInstance } from '../../_models/axisInstance';
import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';

@Component({
  selector: 'app-my-collaborators-sheets',
  templateUrl: './my-collaborators-sheets.component.html',
  styleUrls: ['./my-collaborators-sheets.component.css'],
})
export class MyCollaboratorsSheetsComponent implements OnInit {
  @Input() sheetsToValidate: EvaluationFileInstance[];
  @Input() sheetToValidate: EvaluationFileInstance;
  @Input() goalsByAxisInstanceList: GoalByAxisInstance[];
  @Input() goalsMode: boolean;
  @Input() toggleChangeAxisWeight: boolean;
  @Input() canValidate: boolean;
  @Input() canEvaluate: boolean;
  @Output() updateUserWeightEvent = new EventEmitter<any>();
  @Output() rejectGoalsEvent = new EventEmitter<any>();
  @Output() acceptGoalsEvent = new EventEmitter<any>();
  @Output() loadGoalsEvent = new EventEmitter<any>();
  @Output() switchOffGoalsEvent = new EventEmitter<boolean>();
  @Output() showSheetDetailEvent = new EventEmitter<any>();

  constructor() {}

  ngOnInit() {}

  handleUpdateUserWeight(data: any) {
    this.updateUserWeightEvent.emit(data);
  }

  switchOffGoalsMode(event: boolean) {
    this.switchOffGoalsEvent.emit(event);
  }

  handleRejectGoals(event: any) {
    this.rejectGoalsEvent.emit(event);
  }

  handleAcceptGoals(acceptanceData: any) {
    this.acceptGoalsEvent.emit(acceptanceData);
  }

  handleLoadGoals(loadGoalsData: any) {
    this.loadGoalsEvent.emit(loadGoalsData);
  }

  handleShowSheetDetail(data: any) {
    this.showSheetDetailEvent.emit(data);
  }
}
