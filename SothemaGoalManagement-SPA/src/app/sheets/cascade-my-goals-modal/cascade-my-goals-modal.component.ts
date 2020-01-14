import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';
import { faPlus, faMinus, faTrash } from '@fortawesome/free-solid-svg-icons';

import { Goal } from '../../_models/goal';
import { GoalWithChildren } from '../../_models/goalWithChildren';
import { Evaluator } from '../../_models/evaluator';
import { AdminService } from '../../_services/admin.service';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { splitAtColon } from '@angular/compiler/src/util';

@Component({
  selector: 'app-cascade-my-goals-modal',
  templateUrl: './cascade-my-goals-modal.component.html',
  styleUrls: ['./cascade-my-goals-modal.component.css']
})
export class CascadeMyGoalsModalComponent implements OnInit {
  @Output() cascadeMyGoalEvent = new EventEmitter<any>();
  subGoal: any;
  subGoals: any[] = [];
  filteredSubGoals: any[] = [];
  myGoal: Goal;
  axisInstanceTitle: string;
  public loading = false;
  selectedAll: boolean;
  values: string = '';
  faPlus = faPlus;
  faMinus = faMinus;
  goalWithChildren: GoalWithChildren;
  faTrash = faTrash;
  showSubGoal: boolean;
  goalChildren: GoalWithChildren[] = [];
  sheetStatus: string;

  constructor(public bsModalRef: BsModalRef, private userService: UserService, private adminService: AdminService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMyEvaluatees();
  }

  loadMyEvaluatees() {
    this.loading = true;
    this.adminService
      .loadEvaluatees(this.authService.decodedToken.nameid)
      .subscribe(
        (evaluatees: Evaluator[]) => {
          this.loading = false;
          evaluatees.forEach(evaluatee => this.constructData(evaluatee));
          this.filteredSubGoals = this.subGoals;
          this.loadGoalDetail();
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadGoalDetail() {
    this.loading = true;
    this.userService.getGoalDetail(this.authService.decodedToken.nameid, this.myGoal.id)
      .subscribe((res: GoalWithChildren) => {
        this.loading = false;
        this.goalWithChildren = res;
        this.goalChildren = this.goalWithChildren.children;
      },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  deleteGoal(subGoal: GoalWithChildren) {
    this.alertify.confirm('Supprimer',
      `Êtes-vous sûr de vouloir supprimer le sous-objectif: ${subGoal.description}, assigné à ${subGoal.ownerFullName}?`,
      () => {
        this.loading = true;
        this.userService
          .deleteGoal(subGoal.id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.loading = false;
              this.goalWithChildren.children.splice(
                this.goalWithChildren.children.findIndex(a => a.id === subGoal.id),
                1
              );
              this.alertify.success('Le sous-objectif a été supprimé');
            },
            error => {
              this.loading = false;
              this.alertify.error(error);
            }
          );
      }
    );
  }

  toggleSubGoal(subGoal: GoalWithChildren) {
    this.showSubGoal = !this.showSubGoal;
  }

  constructData(evaluatee: Evaluator) {
    this.subGoal = {
      selected: false,
      evaluateeId: evaluatee.id,
      fullName: evaluatee.fullName,
      description: this.myGoal.description,
      weight: 0
    };
    this.subGoals.push(this.subGoal);
  }

  selectAll() {
    this.selectedAll = !this.selectedAll;
    for (var i = 0; i < this.filteredSubGoals.length; i++) {
      this.filteredSubGoals[i].selected = this.selectedAll;
    }
  }

  checkIfAllSelected() {
    var totalSelected = 0;
    for (var i = 0; i < this.filteredSubGoals.length; i++) {
      if (this.filteredSubGoals[i].selected) totalSelected++;
    }
    this.selectedAll = totalSelected === this.filteredSubGoals.length;

    return true;
  }

  cascadeGoal() {
    const { id, axisInstance, goalType, projectName, status } = this.myGoal;
    let golasForCascade = this.filteredSubGoals.filter(sg => sg.selected == true)
      .map(ssg => ({
        evaluateeId: ssg.evaluateeId,
        goalForCreationDto: {
          description: ssg.description,
          axisInstanceId: axisInstance.id,
          goalTypeId: goalType.id,
          projectName,
          weight: ssg.weight,
          status: status
        },
        parentGoalId: id,
        axisInstanceTitle: this.axisInstanceTitle
      }));
    this.cascadeMyGoalEvent.emit(golasForCascade);
    this.bsModalRef.hide();
  }

  disableAction() {
    if (this.filteredSubGoals === undefined) return true;
    for (var i = 0; i < this.filteredSubGoals.length; i++) {
      if (this.filteredSubGoals[i].selected) {
        return false;
      }
    }
    return true;
  }

  setDescriptionOrWeight(idx: number, event) {
    this.filteredSubGoals[idx][event.target.name] = event.target.value;
  }

  onKeyUp(event) {
    this.values = event.target.value;
    this.filteredSubGoals = this.subGoals.filter(sb => sb.fullName.toLowerCase().includes(this.values.toLowerCase()));
  }

  addSubGoal(idx, subGoal) {
    const sg = {
      selected: true,
      evaluateeId: subGoal.evaluateeId,
      fullName: subGoal.fullName,
      description: subGoal.description,
      weight: 0
    };
    this.filteredSubGoals.splice(idx, 0, sg);
  }

  removeSubGoal(idx, evaluateeId) {
    if (this.filteredSubGoals.filter(sb => sb.evaluateeId === evaluateeId).length > 1) {
      this.filteredSubGoals.splice(idx, 1);
    }
  }

  disableRemoveSubGoal(evaluateeId) {
    return this.filteredSubGoals.filter(sb => sb.evaluateeId === evaluateeId).length == 1;
  }
}
