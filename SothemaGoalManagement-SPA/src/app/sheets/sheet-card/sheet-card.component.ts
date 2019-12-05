import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faCaretDown, faCaretUp, faCheckSquare, faEye, faList } from '@fortawesome/free-solid-svg-icons';

import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { AxisInstance } from '../../_models/axisInstance';
import { AlertifyService } from '../../_services/alertify.service';
import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';

@Component({
  selector: 'app-sheet-card',
  templateUrl: './sheet-card.component.html',
  styleUrls: ['./sheet-card.component.css']
})
export class SheetCardComponent implements OnInit {
  @Input() sheetToValidate: EvaluationFileInstance;
  @Output() updateUserWeightEvent = new EventEmitter<any>();
  @Output() loadGoalsEvent = new EventEmitter<any>();
  @Output() showSheetDetailEvent = new EventEmitter<EvaluationFileInstance>();
  axisInstanceList: AxisInstance[];
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;
  faCheckSquare = faCheckSquare;
  faEye = faEye;
  faList = faList;
  isCollapsed = false;
  message: string = '';
  goalsStatus: string = '';

  constructor(private alertify: AlertifyService) { }

  ngOnInit() {
    this.axisInstanceList = this.sheetToValidate.axisInstances;
  }

  handleUpdateUserWeight(data) {
    this.updateUserWeightEvent.emit(data);
    this.tallyUserWeights();

  }

  toggleAxis() {
    this.tallyUserWeights();
    this.isCollapsed = !this.isCollapsed;
  }
  showGoals() {
    var axisInstanceIds = this.sheetToValidate.axisInstances.map(a => a.id);
    var loadGoalsData = { sheetToValidate: this.sheetToValidate, axisInstanceIds: axisInstanceIds };
    this.loadGoalsEvent.emit(loadGoalsData);
  }

  showSheetDetail() {
    this.showSheetDetailEvent.emit(this.sheetToValidate);
  }

  tallyUserWeights() {
    let totalWeights = this.sheetToValidate.axisInstances.reduce((accumWeights, axisInstance) => accumWeights + (typeof axisInstance.userWeight === 'string' ? parseInt(axisInstance.userWeight) : axisInstance.userWeight), 0);
    if (totalWeights !== 100) {
      this.message = `Pondération Utilisateur total est égale à ${totalWeights}%, elle doit être égale à 100%.`;
    } else {
      this.message = '';
    }
  }
}
