import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';;
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { TabsetComponent } from 'ngx-bootstrap';
import { Subject, combineLatest } from 'rxjs';

import { Pagination, PaginatedResult } from '../../_models/pagination';
import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Goal } from '../../_models/goal';
import { GoalEditModalComponent } from '../goal-edit-modal/goal-edit-modal.component';
import { AxisInstance } from '../../_models/axisInstance';
import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';


@Component({
  selector: 'app-sheets-panel',
  templateUrl: './sheets-panel.component.html',
  styleUrls: ['./sheets-panel.component.css']
})
export class SheetsPanelComponent implements OnInit {
  private ngUnsubscribe = new Subject();
  private tabSetInitialized = new Subject();
  public tabSet: TabsetComponent;
  @ViewChild('tabset') tabset: TabsetComponent;
  pagination: Pagination;
  sheets: EvaluationFileInstance[];
  sheetToValidate: EvaluationFileInstance;
  sheetsToValidate: EvaluationFileInstance[];
  public loading = false;
  goalList: Goal[];
  bsModalRef: BsModalRef;
  goalsByAxisInstanceList: GoalByAxisInstance[];
  goalsMode = false;
  detailMode: boolean;
  tabIndex: number = 0;
  filters: any = {};
  statusList: string[];
  toggleChangeAxisWeight: boolean;


  public behavioralSkillEvaluationUpdated: boolean;

  constructor(private modalService: BsModalService, private route: ActivatedRoute, private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.statusList = ['Rédaction', 'En Revue', 'Publiée', 'Archivée'];
    this.filters.status = '';
    this.route.data.subscribe(data => {
      const resolvedData = data['resolvedData'];
      this.sheetsToValidate = resolvedData['sheetsToValidate'];
      this.sheets = resolvedData['sheets'].result;
      this.pagination = resolvedData['sheets'].pagination;
      this.checkParameters();
    });
  }

  loadSheets() {
    this.loading = true;
    this.userService
      .getMySheets(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.filters
      )
      .subscribe(
        (res: PaginatedResult<EvaluationFileInstance[]>) => {
          this.loading = false;
          this.sheets = res.result;
          this.pagination = res.pagination;
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  checkParameters() {

    this.sheetsToValidate.forEach(sheetToValidate => {
      if (sheetToValidate.parameters.length > 0) {
        let changeAxisWeightIdx = sheetToValidate.parameters.findIndex(p => p.event.includes('Change Axis Weight'));
        if (changeAxisWeightIdx > -1) {
          this.toggleChangeAxisWeight = sheetToValidate.parameters[changeAxisWeightIdx].toggleChangeAxisWeight;
        } else {
          this.toggleChangeAxisWeight = false;
        }
      }
    });


    console.log('toggleChangeAxisWeight:', this.toggleChangeAxisWeight);
  }
  loadSheetsToValidate() {
    this.loading = true;
    this.userService.getMyCollaboratorsSheets(
      this.authService.decodedToken.nameid, this.filters
    )
      .subscribe(
        (res: EvaluationFileInstance[]) => {
          this.loading = false;
          this.sheetsToValidate = res;
          this.checkParameters();
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleDeleteGoal(id: number) {
    this.alertify.confirm('Supprimer',
      'Êtes-vous sûr de vouloir supprimer cet objectif?',
      () => {
        this.loading = true;
        this.userService
          .deleteGoal(id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.loading = false;
              this.goalList.splice(
                this.goalList.findIndex(a => a.id === id),
                1
              );
              this.alertify.success('L\'objectif a été supprimé');
            },
            error => {
              this.loading = false;
              this.alertify.error(error);
            }
          );
      }
    );
  }

  editAxisModal(goal: Goal) {
    const initialState = {
      goal
    };

    this.bsModalRef = this.modalService.show(GoalEditModalComponent, { initialState, ignoreBackdropClick: true });
    this.bsModalRef.content.updateSelectedGoal.subscribe((updatedGoal) => {
      this.loading = true;
      this.userService.updateGoal(goal.id, this.authService.decodedToken.nameid, updatedGoal).subscribe(() => {
        this.loading = false;
        this.alertify.success('L\'objectif été mis à jour.');
      }, error => {
        this.loading = false;
        this.alertify.error(error);
      })

    });
  }

  handlePageChanged(event: any): void {
    this.pagination.currentPage = event.currentPage;
    this.loadSheets();
  }

  handleUpdateUserWeight({ oldUserWeight, axisInstance }) {
    this.loading = true;
    this.userService
      .updateAxisInstance(
        this.authService.decodedToken.nameid,
        axisInstance.id,
        axisInstance.userWeight
      )
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success('La pondération de l\'employée est modifié avec succès.');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
          const sheetToValidate = this.sheetsToValidate.find(s => s.axisInstances.includes(axisInstance));
          const idx = sheetToValidate.axisInstances.findIndex(ai => ai.id === axisInstance.id);
          sheetToValidate.axisInstances[idx].userWeight = oldUserWeight;
          this.sheetToValidate = sheetToValidate;
        }
      );
  }

  handleRejectGoals(event: any) {
    this.loading = true;
    this.userService
      .validateGoals(this.authService.decodedToken.nameid, event.sheetId, event.goals)
      .subscribe(
        () => {
          this.loading = false;
          this.loadSheetsToValidate();
          this.alertify.success('Les objectives ont été renvoyées');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleAcceptGoals(acceptanceData: any) {
    this.loading = true;
    this.userService
      .validateGoals(this.authService.decodedToken.nameid, acceptanceData.sheetId, acceptanceData.goals)
      .subscribe(
        () => {
          this.loading = false;
          this.loadSheetsToValidate();
          this.alertify.success('Les objectives ont été validées');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleLoadGoals(loadGoalsData: any) {

    this.loading = true;
    this.userService.getGoalsForAxis(this.authService.decodedToken.nameid, loadGoalsData.axisInstanceIds).subscribe(
      (res: GoalByAxisInstance[]) => {
        this.loading = false;
        this.goalsByAxisInstanceList = res;
        this.sheetToValidate = loadGoalsData.sheetToValidate;
        this.goalsMode = true;
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  switchOffGoalsMode(event: boolean) {
    this.goalsMode = event;
  }

  handleShowSheetDetail(data: any) {
    this.sheetToValidate = data.sheet;
    this.tabIndex = data.tab;
    this.detailMode = true;
  }

  switchOffDetailMode() {
    this.detailMode = false;
  }

  handleBehavioralSkillEvaluationUpdated(event: boolean) {
    this.behavioralSkillEvaluationUpdated = event;
  }

  loadData() {
    this.loadSheets();
    this.loadSheetsToValidate();
  }

  resetFilters() {
    this.filters.status = '';
    this.loadData();
  }
}
