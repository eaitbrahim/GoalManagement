import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';

import { Goal } from '../../_models/goal';
import { GoalType } from '../../_models/goalType';
import { Axis } from '../../_models/axis';
import { GoalByAxisInstance } from './../../_models/goalsByAxisInstance';
import { Project } from '../../_models/project';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';

@Component({
  selector: 'app-goal-edit-modal',
  templateUrl: './goal-edit-modal.component.html',
  styleUrls: ['./goal-edit-modal.component.css']
})
export class GoalEditModalComponent implements OnInit {
  @Output() editGoalEvent = new EventEmitter<any>();
  goalsByAxisInstance: GoalByAxisInstance;
  goal: Goal;
  axisList: Axis[];
  goalTypeList: GoalType[];
  projectList: Project[];
  updatedGoal: any = {};
  showError: boolean = false;
  filteredProjects: Project[] = [];
  public loading = false;

  constructor(public bsModalRef: BsModalRef, private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.updatedGoal = {
      'id': this.goal.id,
      'description': this.goal.description,
      'axisInstanceId': this.goal.axisInstance.id,
      'goalTypeId': this.goal.goalType.id,
      'projectName': this.goal.projectName,
      'weight': this.goal.weight,
      'status': this.goal.status
    }

    if (!this.goalTypeList) this.loadGoalTypes();
    if (!this.projectList) this.loadProjects();
  }

  updateGoal() {
    if (this.isTotalWeightValid()) {
      if (this.updatedGoal.goalTypeId > 3) {
        this.updatedGoal.projectName = '';
      }

      this.editGoalEvent.emit(this.updatedGoal);
      this.bsModalRef.hide();
    } else {
      this.showError = true;
    }
  }

  isTotalWeightValid() {
    let totalWeight = this.updatedGoal.weight;
    this.goalsByAxisInstance.goals.forEach(goal => {
      if (this.updatedGoal.id != goal.id) {
        totalWeight = totalWeight + goal.weight;
      }
    });

    if (totalWeight > 100) {
      return false;
    }
    return true;
  }

  onChange($event) {
    this.showError = false;
  }

  onChangeGoalType() {
    this.filteredProjects = this.projectList.filter(p => p.goalTypeId === this.updatedGoal.goalTypeId);
    this.filteredProjects.length > 0 ? this.updatedGoal.projectName = this.filteredProjects[0].name : '';
  }

  loadGoalTypes() {
    this.loading = true;
    this.userService.getGoalTypes(this.authService.decodedToken.nameid).subscribe(
      (res: GoalType[]) => {
        this.loading = false;
        this.goalTypeList = res;
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  loadProjects() {
    this.userService.getProjects(this.authService.decodedToken.nameid).subscribe(
      (res: Project[]) => {
        this.loading = false;
        this.projectList = res;
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }
}
