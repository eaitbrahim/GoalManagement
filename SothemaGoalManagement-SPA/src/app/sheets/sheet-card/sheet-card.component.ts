import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {
  faCaretDown,
  faCaretUp,
  faCheckSquare,
  faEye,
  faList,
  faHandRock,
} from '@fortawesome/free-solid-svg-icons';

import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { AxisInstance } from '../../_models/axisInstance';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-sheet-card',
  templateUrl: './sheet-card.component.html',
  styleUrls: ['./sheet-card.component.css'],
})
export class SheetCardComponent implements OnInit {
  @Input() sheetToValidate: EvaluationFileInstance;
  @Input() canDoFinalEvaluation: boolean;
  @Input() refreshWeight: boolean;
  @Output() updateUserWeightEvent = new EventEmitter<any>();
  @Output() loadGoalsEvent = new EventEmitter<any>();
  @Output() showSheetDetailEvent = new EventEmitter<any>();
  toggleChangeAxisWeight = true;
  canValidate = true;
  canEvaluate = true;
  axisInstanceList: AxisInstance[];
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;
  faCheckSquare = faCheckSquare;
  faHandRock = faHandRock;
  faEye = faEye;
  faList = faList;
  isCollapsed = false;
  message = '';
  goalsStatus = '';

  constructor(private alertify: AlertifyService) {}

  ngOnInit() {
    this.checkParameters();
    this.axisInstanceList = this.sheetToValidate.axisInstances;
    console.log('this.refreshWeight=', this.refreshWeight);
    if (this.refreshWeight) {
      this.tallyUserWeights();
    }
  }

  checkParameters() {

      if (this.sheetToValidate.parameters.length > 0) {
        // Check if user can update axis weight
        this.toggleChangeAxisWeight = this.checkEvents(this.sheetToValidate, 'Change Axis Weight');

        // Check user can validate goals
          this.canValidate = this.checkEvents(this.sheetToValidate, 'Plage de dates de validation des objectifs');

        // Check user can evaluate goals
          this.canEvaluate = this.checkEvents(this.sheetToValidate, `Plage de dates d'évaluation`);

        // Check user can do final evaluation
        this.canDoFinalEvaluation = this.checkEvents(this.sheetToValidate, `Plage de dates de validation finale de la fiche`);
      }
  }

  checkEvents(sheetToValidate, event) {
    const eventDateIdx = sheetToValidate.parameters.findIndex((p) => p.event.includes(event));

    if (eventDateIdx > -1) {
      if (event === 'Change Axis Weight') {
        return sheetToValidate.parameters[eventDateIdx].toggleChangeAxisWeight;
      }

      const from = new Date(sheetToValidate.parameters[eventDateIdx].startEvent);
      const to = new Date(sheetToValidate.parameters[eventDateIdx].endEvent);
      to.setDate(to.getDate() + 1);
      const dateCheck = new Date();

      return dateCheck >= from && dateCheck < to;
    }

    if (event === 'Change Axis Weight') {
      return false;
    } else {
      return true;
    }
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
    const axisInstanceIds = this.sheetToValidate.axisInstances.map((a) => a.id);
    const loadGoalsData = {
      sheetToValidate: this.sheetToValidate,
      axisInstanceIds: axisInstanceIds,
    };
    this.loadGoalsEvent.emit(loadGoalsData);
  }

  showSheetDetail(tab: number) {
    const data = { tab, sheet: this.sheetToValidate, canDoFinalEvaluation: this.canDoFinalEvaluation, canEvaluate: this.canEvaluate };
    this.showSheetDetailEvent.emit(data);
  }

  tallyUserWeights() {
    const totalWeights = this.sheetToValidate.axisInstances.reduce(
      (accumWeights, axisInstance) =>
        accumWeights +
        (typeof axisInstance.userWeight === 'string'
          ? parseInt(axisInstance.userWeight, 10)
          : axisInstance.userWeight),
      0
    );
    if (totalWeights !== 100) {
      this.message = `Pondération Utilisateur total doit être égale à 100%.`;
    } else {
      this.message = '';
    }
  }
}
