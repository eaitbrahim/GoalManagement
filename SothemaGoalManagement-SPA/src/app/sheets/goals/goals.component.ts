import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { faPlus, faCaretUp, faListUl, faClipboardCheck } from '@fortawesome/free-solid-svg-icons';

import { AxisInstance } from '../../_models/axisInstance';
import { GoalByAxisInstance } from './../../_models/goalsByAxisInstance';
import { Goal } from '../../_models/goal';
import { GoalType } from '../../_models/goalType';
import { Project } from '../../_models/project';

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrls: ['./goals.component.css']
})
export class GoalsComponent implements OnInit {
  @Input() axisInstances: AxisInstance[];
  @Input() golasActions: boolean;
  @Input() approbationAction: boolean;
  @Input() sheetId: number;
  @Input() areGoalsCompleted: boolean;
  @Input() goalsByAxisInstanceList: GoalByAxisInstance[];
  @Input() goalTypeList: GoalType[];
  @Input() projectList: Project[];
  @Input() sheetStatus: string;
  @Output() createGoalEvent = new EventEmitter<any>();
  @Output() editGoalEvent = new EventEmitter<Goal>();
  @Output() deleteGoalEvent = new EventEmitter<Goal>();
  @Output() validateGoalsEvent = new EventEmitter();
  @Output() cascadeMyGoalEvent = new EventEmitter<any>();
  isCollapsed = false;
  faPlus = faPlus;
  faCaretUp = faCaretUp;
  faList = faListUl;
  faCheck = faClipboardCheck;

  constructor() { }

  ngOnInit() {
    console.log('golasActions in goals On Init:', this.golasActions);
  }

  ngOnChanges() {
    console.log('golasActions in goals On Changes:', this.golasActions);
  }

  handleCreateGoal(newGoal: any) {
    this.createGoalEvent.emit(newGoal);
  }

  handleEditGoal(goal: any) {
    this.editGoalEvent.emit(goal);
  }

  handleDeleteGoal(goal: Goal) {
    this.deleteGoalEvent.emit(goal);
  }

  validateGoals() {
    this.validateGoalsEvent.emit()
  }

  handleCascadeMyGoal(golasForCascade: any) {
    this.cascadeMyGoalEvent.emit(golasForCascade);
  }
}
