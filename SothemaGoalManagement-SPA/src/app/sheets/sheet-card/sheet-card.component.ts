import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import {
  faCaretDown,
  faCaretUp,
  faCheckSquare,
  faEye,
  faList,
  faHandRock,
} from "@fortawesome/free-solid-svg-icons";

import { EvaluationFileInstance } from "../../_models/evaluationFileInstance";
import { AxisInstance } from "../../_models/axisInstance";
import { AlertifyService } from "../../_services/alertify.service";
import { GoalByAxisInstance } from "../../_models/goalsByAxisInstance";

@Component({
  selector: "app-sheet-card",
  templateUrl: "./sheet-card.component.html",
  styleUrls: ["./sheet-card.component.css"],
})
export class SheetCardComponent implements OnInit {
  @Input() sheetToValidate: EvaluationFileInstance;
  @Input() toggleChangeAxisWeight: boolean;
  @Output() updateUserWeightEvent = new EventEmitter<any>();
  @Output() loadGoalsEvent = new EventEmitter<any>();
  @Output() showSheetDetailEvent = new EventEmitter<any>();
  axisInstanceList: AxisInstance[];
  faCaretDown = faCaretDown;
  faCaretUp = faCaretUp;
  faCheckSquare = faCheckSquare;
  faHandRock = faHandRock;
  faEye = faEye;
  faList = faList;
  isCollapsed = false;
  message = "";
  goalsStatus = "";

  constructor(private alertify: AlertifyService) {}

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
    const axisInstanceIds = this.sheetToValidate.axisInstances.map((a) => a.id);
    const loadGoalsData = {
      sheetToValidate: this.sheetToValidate,
      axisInstanceIds: axisInstanceIds,
    };
    this.loadGoalsEvent.emit(loadGoalsData);
  }

  showSheetDetail(tab: number) {
    const data = { tab, sheet: this.sheetToValidate };
    this.showSheetDetailEvent.emit(data);
  }

  tallyUserWeights() {
    const totalWeights = this.sheetToValidate.axisInstances.reduce(
      (accumWeights, axisInstance) =>
        accumWeights +
        (typeof axisInstance.userWeight === "string"
          ? parseInt(axisInstance.userWeight)
          : axisInstance.userWeight),
      0
    );
    if (totalWeights !== 100) {
      this.message = `Pondération Utilisateur total doit être égale à 100%.`;
    } else {
      this.message = "";
    }
  }
}
