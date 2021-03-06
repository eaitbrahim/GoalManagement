import { BehavioralSkillEvaluation } from './../../_models/behavioralSkillEvaluation';
import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewChild,
} from '@angular/core';
import { TabsetComponent } from 'ngx-bootstrap';
import { ActivatedRoute, Router } from '@angular/router';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';

import { EvaluationFileInstance } from '../../_models/evaluationFileInstance';
import { Goal } from '../../_models/goal';
import { GoalType } from '../../_models/goalType';
import { HrService } from '../../_services/hr.service';
import { UserService } from '../../_services/user.service';
import { AdminService } from '../../_services/admin.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { GoalByAxisInstance } from '../../_models/goalsByAxisInstance';
import { BehavioralSkillInstance } from '../../_models/behavioralSkillInstance';
import { Project } from '../../_models/project';
import { Evaluator } from '../../_models/evaluator';
import { Parameters } from '../../_models/parameters';
import { User } from '../../_models/user';

@Component({
  selector: 'app-sheet-detail',
  templateUrl: './sheet-detail.component.html',
  styleUrls: ['./sheet-detail.component.css'],
})
export class SheetDetailComponent implements OnInit {
  @ViewChild('sheetTabs') sheetTabs: TabsetComponent;
  @Input() sheetToValidate: EvaluationFileInstance;
  @Input() tabIndex: number;
  @Input() canDoFinalEvaluation: boolean;
  @Input() canEvaluate: boolean;
  @Output() switchOffDetailModeEvent = new EventEmitter();
  @Output() behavioralSkillEvaluationUpdatedEvent = new EventEmitter<boolean>();
  sheetDetail: EvaluationFileInstance;
  goalsByAxisInstanceList: GoalByAxisInstance[];
  behavioralSkillInstanceList: BehavioralSkillInstance[];
  goalTypeList: GoalType[];
  projectList: Project[];
  public loading = false;
  areGoalsCompleted: boolean;
  golasActions = false;
  approbationAction = false;
  areGoalsEvaluable: boolean;
  areBehavioralSkillsEvaluable = false;
  totalGrade: string;
  behavioralSkillEvaluationUpdated: boolean;
  showDetail: boolean;
  faArrowLeft = faArrowLeft;
  evaluators: Evaluator[] = [];
  parameters: Parameters[] = [];
  validatorFullName: string;
  toggleChangeAxisWeight: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private hrService: HrService,
    private adminService: AdminService,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    console.log(
      '(On Init sheet detail) golasActions:',
      this.golasActions
    );
    if (this.sheetToValidate) {
      this.sheetDetail = this.sheetToValidate;
      this.sheetTabs.tabs[this.tabIndex].active = true;
      this.getValidatorFullName();
      this.loadParameters();
      this.fetchEvaluators();
      this.getGoalsForAxis();
      this.getBehavioralSkillInstances();
      this.loadGoalTypes();
      this.loadProjects();
    } else {
      this.route.data.subscribe((data) => {
        const resolvedData = data['resolvedData'];
        this.sheetDetail = resolvedData['sheetDetail'];
        this.goalTypeList = resolvedData['goalTypeList'];
        this.projectList = resolvedData['projectList'];
        this.getValidatorFullName();
        this.loadParameters();
        this.fetchEvaluators();
        this.getGoalsForAxis();
        this.getBehavioralSkillInstances();
      });
    }
  }

  getGoalsForAxis() {
    const axisInstanceIds = this.sheetDetail.axisInstances.map((a) => a.id);
    this.loading = true;
    this.userService
      .getGoalsForAxis(this.sheetDetail.ownerId, axisInstanceIds)
      .subscribe(
        (result: GoalByAxisInstance[]) => {
          this.loading = false;
          this.goalsByAxisInstanceList = result;
          this.CheckReadOnly();
          this.CanGoalsBeValidated();
          this.CanGoalsBeEvaluated();
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  getBehavioralSkillInstances() {
    this.loading = true;
    this.userService
      .getBehavioralSkillInstances(
        this.sheetDetail.ownerId,
        this.sheetDetail.id
      )
      .subscribe(
        (result: BehavioralSkillInstance[]) => {
          this.loading = false;
          this.CanBehavioralSkillBeEvaluated();
          this.behavioralSkillInstanceList = result;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  CheckReadOnly() {
    if (this.checkEvents(`Plage de dates de validation des objectifs`)) {
      if (this.sheetDetail.ownerId === parseInt(this.authService.decodedToken.nameid, 10)) {
        console.log('Owner');
        const goalsInInitialStatus = this.goalsByAxisInstanceList.filter(
          (g) => g.goalsStatus === 'Pas encore créé' || g.goalsStatus === 'Rédaction'
        );

        if (goalsInInitialStatus.length !== 0) {
          this.golasActions = true;
          console.log(
            '(Initial Status) golasActions:',
            this.golasActions
          );
        }
      }
    }

    this.approbationAction = this.golasActions;
    if (this.authService.roleMatch(['HRD'])) {
      this.golasActions = true;
      this.approbationAction = false;
      console.log('(HRD) golasActions:', this.golasActions);
    }

    return this.golasActions;
  }

  handleCreateGoal(newGoal: any) {
    this.loading = true;
    this.userService
      .createGoal(
        this.authService.decodedToken.nameid,
        this.sheetDetail.id,
        newGoal
      )
      .subscribe(
        () => {
          this.loading = false;
          this.getGoalsForAxis();
          this.alertify.success('Objectif créé avec succès.');
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleEditGoal(goal: any) {
    this.loading = true;
    this.userService
      .updateGoal(goal.id, this.authService.decodedToken.nameid, goal)
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success('Objectif a été mis à jour.');
          this.getGoalsForAxis();
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleDeleteGoal(goal: Goal) {
    this.alertify.confirm(
      'Supprimer',
      `Êtes-vous sûr de vouloir supprimer l'objectif: ${goal.description}?`,
      () => {
        this.loading = true;
        this.userService
          .deleteGoal(goal.id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.loading = false;
              this.getGoalsForAxis();
              this.alertify.success('L\'objectif a été supprimée');
            },
            (error) => {
              this.loading = false;
              this.alertify.error(error);
            }
          );
      }
    );
  }

  CanGoalsBeValidated() {
    console.log('this.goalsByAxisInstanceList:', this.goalsByAxisInstanceList);
    if (this.goalsByAxisInstanceList.filter((g) => g.totalGoalWeight !== 100).length === 0) {
      this.areGoalsCompleted = true;
      if (this.parameters.length > 0) {
        this.areGoalsCompleted = this.checkEvents(`Plage de dates d'évaluation`);
      }

      if (this.sheetDetail.status === 'Publiée') {
        this.areGoalsCompleted = true;
      }
    } else {
      this.areGoalsCompleted = false;
    }

    console.log('areGoalsCompleted:', this.areGoalsCompleted);
    return this.areGoalsCompleted;
  }

  CanGoalsBeEvaluated() {
    if (!this.canEvaluate) {
      this.areGoalsEvaluable = false;
      console.log('Can not evaluate goals!');
      return this.areGoalsEvaluable;
    } else {
      this.areGoalsEvaluable = true;
    }

    if (this.goalsByAxisInstanceList.filter((g) => g.goalsStatus === 'Publiée').length === 0) {
      this.areGoalsEvaluable = false;
    } else {
      this.areGoalsEvaluable = true;
    }

    if (this.sheetDetail.status === 'Publiée') {
      this.areGoalsEvaluable = true;
    }

    console.log('this.sheetDetail.status:', this.sheetDetail.status);
    return this.areGoalsEvaluable;
  }

  CanBehavioralSkillBeEvaluated() {
    const indx = this.evaluators.findIndex(
      (e) => e.id === parseInt(this.authService.decodedToken.nameid, 10)
    );
    if (indx !== -1) {
      if (this.parameters.length > 0 && this.sheetDetail.status !== 'Publiée') {
        this.areBehavioralSkillsEvaluable = this.checkEvents(`Plage de dates d'évaluation`);
      }
    }
  }

  fetchEvaluators() {
    this.adminService.loadEvaluators(this.sheetDetail.ownerId).subscribe(
      (result) => {
        this.evaluators = result;
      },
      (error) => {
        console.log('error in CanBehavioralSkillBeEvaluated: ', error);
      }
    );
  }

  handleValidateGoals() {
    this.loading = true;

    const goals: any[] = [];
    this.goalsByAxisInstanceList.forEach((a) => {
      a.goals.forEach((g) =>
        goals.push({
          id: g.id,
          description: this.sheetDetail.title, // For logs
          goalTypeId: g.goalType.id,
          axisInstanceId: g.axisInstance.id,
          weight: g.weight,
          status: 'En Revue',
          sheetTitle: this.sheetDetail.title,
          emailContent: `S'il vous plaît valider les objectifs pour la fiche d'évaluation ${this.sheetDetail.title}.`,
          sheetOwnerId: this.sheetDetail.ownerId,
        })
      );
    });

    this.userService
      .validateGoals(
        this.authService.decodedToken.nameid,
        this.sheetDetail.id,
        goals
      )
      .subscribe(
        () => {
          this.loading = false;
          this.getGoalsForAxis();
          this.alertify.success(
            'Les objectifs ont été envoyées pour validation'
          );
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleAddGoalEvaluation(newEval: any) {
    this.loading = true;
    const goalEval = {
      ...newEval,
      evaluatorId: this.authService.decodedToken.nameid,
    };
    this.userService
      .addGoalEvaluations(this.sheetDetail.ownerId, goalEval)
      .subscribe(
        () => {
          this.loading = false;
          this.getGoalsForAxis();
          this.alertify.success('L\'évaluation a été ajoutée avec succès.');
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleAddBehavioralSkillEvaluation(evals: any[]) {
    this.loading = true;
    this.userService
      .addBehavioralSkillEvaluations(this.sheetDetail.ownerId, evals)
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success(
            'les évaluations de compétences comportementales ont été enregistrées avec succès.'
          );
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  handleBehavioralSkillEvaluationUpdated(event: boolean) {
    this.behavioralSkillEvaluationUpdated = event;
    this.behavioralSkillEvaluationUpdatedEvent.emit(event);
  }

  returnToList() {
    if (this.sheetToValidate) {
      if (this.behavioralSkillEvaluationUpdated) {
        this.alertify.confirm(
          'Confirmer',
          `Êtes-vous sûr de vouloir revenir à la liste avant d’enregistrer les modifications apportées aux compétences comportementales?`,
          () => {
            this.switchOffDetailModeEvent.emit();
            this.behavioralSkillEvaluationUpdated = false;
            this.behavioralSkillEvaluationUpdatedEvent.emit(false);
          }
        );
      } else {
        this.switchOffDetailModeEvent.emit();
      }
    } else {
      this.router.navigate(['/sheets']);
    }
  }

  handleCascadeMyGoal(golasForCascade: any) {
    this.loading = true;
    this.userService
      .cascadeGoal(
        this.authService.decodedToken.nameid,
        golasForCascade,
        this.sheetDetail.id,
        this.sheetDetail.evaluationFileId
      )
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success(
            'Votre demande de création de sous-objectifs a été envoyée avec succès. Veuillez vérifier vos messages pour les résultats.'
          );
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadGoalTypes() {
    this.loading = true;
    this.userService
      .getGoalTypes(this.authService.decodedToken.nameid)
      .subscribe(
        (res: GoalType[]) => {
          this.loading = false;
          this.goalTypeList = res;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  loadProjects() {
    this.userService
      .getProjects(this.authService.decodedToken.nameid)
      .subscribe(
        (res: Project[]) => {
          this.loading = false;
          this.projectList = res;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
          console.log('Problem with loading projects.');
        }
      );
  }

  loadParameters() {
    this.loading = true;
    this.hrService.loadParameters(this.sheetDetail.evaluationFileId).subscribe(
      (result: Parameters[]) => {
        this.loading = false;
        this.parameters = result;
        if (this.parameters.length > 0) {
          this.toggleChangeAxisWeight = this.checkEvents('Change Axis Weight');
          // Check user can evaluate goals
            this.canEvaluate = this.checkEvents(`Plage de dates d'évaluation`);

          // Check user can do final evaluation
          this.canDoFinalEvaluation = this.checkEvents(`Plage de dates de validation finale de la fiche`);
          console.log('this.canDoFinalEvaluation:', this.canDoFinalEvaluation);
        }
      },
      (error) => {
        this.loading = false;
        this.alertify.error(error);
        console.log('Impossible de charger les événements.');
      }
    );
  }

  checkEvents(event) {
    console.log('Check event:', event);
    const eventDateIdx = this.parameters.findIndex((p) => p.event.includes(event));

    if (eventDateIdx > -1) {
      if (event === 'Change Axis Weight') {
        return this.parameters[eventDateIdx].toggleChangeAxisWeight;
      }

      const from = new Date(this.parameters[eventDateIdx].startEvent);
      const to = new Date(this.parameters[eventDateIdx].endEvent);
      to.setDate(to.getDate() + 1);
      const dateCheck = new Date();

      return dateCheck >= from && dateCheck < to;
    }

    if (event === 'Change Axis Weight') {
      return false;
    } else {
      console.log('Event not set');
      return true;
    }
  }

  handleAddFinalEvaluation(comment: string) {
    const finalEvaluation: any = {};
    if (
      parseInt(this.authService.decodedToken.nameid, 10) ===
      this.sheetDetail.ownerId
    ) {
      finalEvaluation.ownerComment = comment;
      finalEvaluation.ownerValidationDateTime = new Date();
    } else {
      if (
        this.evaluators.findIndex(
          (e) => e.id === parseInt(this.authService.decodedToken.nameid, 10)
        ) > -1
      ) {
        finalEvaluation.validatorId = this.authService.decodedToken.nameid;
        finalEvaluation.validatorComment = comment;
        finalEvaluation.validatorValidationDateTime = new Date();
      } else {
        return this.alertify.error(
          'Vous n\'êtes pas autorisé à soumettre une évaluation finale pour cette fiche.'
        );
      }
    }

    this.alertify.confirm(
      'Confirmer',
      'Êtes-vous sûr de vouloir ajouter une évaluation finale pour cette fiche?',
      () => {
        this.loading = true;
        this.userService
          .addFinalEvaluation(
            this.authService.decodedToken.nameid,
            this.sheetDetail.id,
            finalEvaluation
          )
          .subscribe(
            () => {
              this.loading = false;
              this.fetchSheet(this.sheetDetail.id);
              this.alertify.success(
                'Votre évaluation finale a été ajoutée avec succès.'
              );
            },
            (error) => {
              this.loading = false;
              this.alertify.error(error);
            }
          );
      }
    );
  }

  fetchSheet(sheetId: number) {
    this.loading = true;
    this.userService
      .getMySheet(sheetId, this.authService.decodedToken.nameid)
      .subscribe(
        (result: EvaluationFileInstance) => {
          this.loading = false;
          this.sheetDetail = result;
          this.getValidatorFullName();
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  getValidatorFullName() {
    if (this.sheetDetail.validatorId > 0) {
      this.loading = true;
      this.userService.getUser(this.sheetDetail.validatorId).subscribe(
        (result: User) => {
          this.loading = false;
          this.validatorFullName = result.firstName + ' ' + result.lastName;
        },
        (error) => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
    }
  }
}
