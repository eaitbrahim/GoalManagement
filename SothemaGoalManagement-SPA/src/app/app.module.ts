import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  BsDropdownModule,
  TabsModule,
  BsDatepickerModule,
  PaginationModule,
  ButtonsModule,
  ModalModule,
  CarouselModule,
  AccordionModule,
  PopoverModule
} from 'ngx-bootstrap';
import { NgxLoadingModule } from 'ngx-loading';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { NgxGalleryModule } from 'ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { TimeAgoPipe } from 'time-ago-pipe';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { CollaboratorListComponent } from './collaborators/collaborator-list/collaborator-list.component';
import { MessagesComponent } from './messages/messages.component';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { UserService } from './_services/user.service';
import { CollaboratorCardComponent } from './collaborators/collaborator-card/collaborator-card.component';
import { CollaboratorDetailComponent } from './collaborators/collaborator-detail/collaborator-detail.component';
import { CollaboratorEditComponent } from './collaborators/collaborator-edit/collaborator-edit.component';
import { CollaboratorListResolver } from './_resolvers/collaborator-list.resolver';
import { CollaboratorDetailResolver } from './_resolvers/collaborator-detail.resolver';
import { CollaboratorNewComponent } from './collaborators/collaborator-new/collaborator-new.component';
import { PreventUnsavedChangesForCollaborator } from './_guards/prevent-unsave-changes-guards-for-collaborator';
import { PreventUnsavedChangesForSheets } from './_guards/prevent-unsave-changes-guards-for-sheets';
import { PhotoEditorComponent } from './collaborators/photo-editor/photo-editor.component';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { CollaboratorMessagesComponent } from './collaborators/collaborator-messages/collaborator-messages.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/hasRole.directive';
import { UserRolesManagementComponent } from './admin/user-roles-management/user-roles-management.component';
import { PhotoManagementComponent } from './admin/photo-management/photo-management.component';
import { AdminService } from './_services/admin.service';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';
import { HrPanelComponent } from './hr/hr-panel/hr-panel.component';
import { StrategiesComponent } from './strategies/strategies.component';
import { StrategiesResolver } from './_resolvers/strategies.resolver';
import { StrategyListComponent } from './hr/strategies/strategy-list/strategy-list.component';
import { StrategyListResolver } from './_resolvers/strategy-list.resolver';
import { HrService } from './_services/hr.service';
import { StrategyNewComponent } from './hr/strategies/strategy-new/strategy-new.component';
import { StrategyDetailComponent } from './hr/strategies/strategy-detail/strategy-detail.component';
import { StrategyEditModalComponent } from './hr/strategies/strategy-edit-modal/strategy-edit-modal.component';
import { StrategyAxisComponent } from './hr/strategies/strategy-axis/strategy-axis.component';
import { StrategyDetailResolver } from './_resolvers/strategy-detail.resolver';
import { ResetPasswordComponent } from './collaborators/reset-password/reset-password.component';
import { ForgetPasswordComponent } from './collaborators/forget-password/forget-password.component';
import { ProfileEditResolver } from './_resolvers/profile-edit.resolver';
import { AxisModalComponent } from './hr/strategies/axis-modal/axis-modal.component';
import { AxisPolesWeightsCardComponent } from './hr/strategies/axis-poles-weights-card/axis-poles-weights-card.component';
import { AxisPoleWeightItemComponent } from './hr/strategies/axis-pole-weight-item/axis-pole-weight-item.component';
import { AxisPolesWeightsListComponent } from './hr/strategies/axis-poles-weights-list/axis-poles-weights-list.component';
import { ProfileEditComponent } from './collaborators/profile-edit/profile-edit.component';
import { StrategyDocumentationComponent } from './hr/strategies/strategy-documentation/strategy-documentation.component';
import { CollaboratorSearchComponent } from './collaborators/collaborator-search/collaborator-search.component';
import { BehavioralSkillNewComponent } from './hr/behavioral-skills/behavioral-skill-new/behavioral-skill-new.component';
import { BehavioralSkillDetailComponent } from './hr/behavioral-skills/behavioral-skill-detail/behavioral-skill-detail.component';
import { BehavioralSkillListComponent } from './hr/behavioral-skills/behavioral-skill-list/behavioral-skill-list.component';
import { BehavioralSkillEditModalComponent } from './hr/behavioral-skills/behavioral-skill-edit-modal/behavioral-skill-edit-modal.component';
import { BehavioralSkillDetailResolver } from './_resolvers/behavioral-skill-detail.resolver';
import { EvaluatorComponent } from './collaborators/evaluator/evaluator.component';
import { EvaluationHrListComponent } from './hr/evaluations/evaluation-hr-list/evaluation-hr-list.component';
import { EvaluationHrEditModalComponent } from './hr/evaluations/evaluation-hr-edit-modal/evaluation-hr-edit-modal.component';
import { EvaluationHrNewComponent } from './hr/evaluations/evaluation-hr-new/evaluation-hr-new.component';
import { EvaluationHrDetailComponent } from './hr/evaluations/evaluation-hr-detail/evaluation-hr-detail.component';
import { HrFilterCreateActionsComponent } from './hr/hr-filter-create-actions/hr-filter-create-actions.component';
import { AdminFilterActionsComponent } from './admin/admin-filter-actions/admin-filter-actions.component';
import { LayoutService } from './_services/layout.service';
import { EvaluationHrDetailResolver } from './_resolvers/evaluation-hr-detail.resolver';
import { EvaluationFileInstanceHrListComponent } from './hr/evaluations/evaluation-file-instance-hr-list/evaluation-file-instance-hr-list.component';
import { EvaluatorAssignmentComponent } from './collaborators/evaluator-assignment/evaluator-assignment.component';
import { EvaluationFileInstanceHrNewComponent } from './hr/evaluations/evaluation-file-instance-hr-new/evaluation-file-instance-hr-new.component';
import { SheetsPanelComponent } from './sheets/sheets-panel/sheets-panel.component';
import { MySheetsListComponent } from './sheets/my-sheets-list/my-sheets-list.component';
import { MyCollaboratorsSheetsComponent } from './sheets/my-collaborators-sheets/my-collaborators-sheets.component';
import { MyCollaboratorGoalsComponent } from './sheets/my-collaborator-goals/my-collaborator-goals.component';
import { SheetsResolver } from './_resolvers/sheets.resolver';
import { SheetDetailComponent } from './sheets/sheet-detail/sheet-detail.component';
import { SheetDetailResolver } from './_resolvers/sheet-detail.resolver';
import { SheetAxisInstanceItemComponent } from './sheets/sheet-axis-instance-item/sheet-axis-instance-item.component';
import { GoalsComponent } from './sheets/goals/goals.component';
import { GoalEditModalComponent } from './sheets/goal-edit-modal/goal-edit-modal.component';
import { GoalNewComponent } from './sheets/goal-new/goal-new.component';
import { GoalCardComponent } from './sheets/goal-card/goal-card.component';
import { SheetCardComponent } from './sheets/sheet-card/sheet-card.component';
import { EvaluationFileInstanceLogsComponent } from './hr/evaluations/evaluation-file-instance-logs/evaluation-file-instance-logs.component';
import { PromptModalComponent } from './prompt-modal/prompt-modal.component';
import { AutoEvaluationComponent } from './sheets/auto-evaluation/auto-evaluation.component';
import { GoalEvaluationModalComponent } from './sheets/goal-evaluation-modal/goal-evaluation-modal.component';
import { CascadeMyGoalsModalComponent } from './sheets/cascade-my-goals-modal/cascade-my-goals-modal.component';
import { AutoEvaluationAxisComponent } from './sheets/auto-evaluation-axis/auto-evaluation-axis.component';
import { BehavioralSkillListEvaluationComponent } from './sheets/behavioral-skill-list-evaluation/behavioral-skill-list-evaluation.component';
import { EvaluationHrParametersComponent } from './hr/evaluations/evaluation-hr-parameters/evaluation-hr-parameters.component';
import { SheetMetaDataComponent } from './sheets/sheet-meta-data/sheet-meta-data.component';
import { FinalValidationComponent } from './sheets/final-validation/final-validation.component';
import { TruncateTextPipe } from './_pipes/truncate-text.pipe';
import { ReportsSheetsComponent } from './reports/reports-sheets/reports-sheets.component';
import { ReportsPanelComponent } from './reports/reports-panel/reports-panel.component';
import { ReportSheetsResolver } from './_resolvers/report-sheets.resolver';
import { ReportsGoalsComponent } from './reports/reports-goals/reports-goals.component';
import { ReportsEvaluationsComponent } from './reports/reports-evaluations/reports-evaluations.component';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    CollaboratorListComponent,
    MessagesComponent,
    CollaboratorCardComponent,
    CollaboratorDetailComponent,
    CollaboratorEditComponent,
    CollaboratorNewComponent,
    CollaboratorMessagesComponent,
    CollaboratorSearchComponent,
    EvaluatorComponent,
    ProfileEditComponent,
    PhotoEditorComponent,
    TimeAgoPipe,
    AdminPanelComponent,
    HasRoleDirective,
    UserRolesManagementComponent,
    PhotoManagementComponent,
    RolesModalComponent,
    MessagesComponent,
    NavComponent,
    HomeComponent,
    HrPanelComponent,
    StrategiesComponent,
    StrategyListComponent,
    StrategyNewComponent,
    StrategyEditModalComponent,
    StrategyDetailComponent,
    StrategyAxisComponent,
    StrategyDocumentationComponent,
    AxisModalComponent,
    AxisPolesWeightsCardComponent,
    AxisPoleWeightItemComponent,
    AxisPolesWeightsListComponent,
    ResetPasswordComponent,
    ForgetPasswordComponent,
    BehavioralSkillListComponent,
    BehavioralSkillDetailComponent,
    BehavioralSkillNewComponent,
    BehavioralSkillEditModalComponent,
    EvaluationHrListComponent,
    EvaluationHrEditModalComponent,
    EvaluationHrNewComponent,
    EvaluationHrDetailComponent,
    EvaluationHrParametersComponent,
    HrFilterCreateActionsComponent,
    AdminFilterActionsComponent,
    EvaluationFileInstanceHrListComponent,
    EvaluatorAssignmentComponent,
    EvaluationFileInstanceHrNewComponent,
    EvaluationFileInstanceLogsComponent,
    SheetsPanelComponent,
    MySheetsListComponent,
    MyCollaboratorsSheetsComponent,
    MyCollaboratorGoalsComponent,
    SheetDetailComponent,
    SheetMetaDataComponent,
    SheetCardComponent,
    SheetAxisInstanceItemComponent,
    GoalsComponent,
    GoalEditModalComponent,
    GoalNewComponent,
    GoalCardComponent,
    PromptModalComponent,
    AutoEvaluationComponent,
    AutoEvaluationAxisComponent,
    GoalEvaluationModalComponent,
    CascadeMyGoalsModalComponent,
    BehavioralSkillListEvaluationComponent,
    FinalValidationComponent,
    TruncateTextPipe,
    ReportsPanelComponent,
    ReportsSheetsComponent,
    ReportsGoalsComponent,
    ReportsEvaluationsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    TabsModule.forRoot(),
    ButtonsModule.forRoot(),
    CarouselModule.forRoot(),
    AccordionModule.forRoot(),
    NgxLoadingModule.forRoot({}),
    PopoverModule.forRoot(),
    RouterModule.forRoot(appRoutes),
    NgxGalleryModule,
    BrowserAnimationsModule,
    ModalModule.forRoot(),
    PaginationModule.forRoot(),
    FileUploadModule,
    FontAwesomeModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ['localhost:5000'],
        blacklistedRoutes: ['localhost:5000/api/auth']
      }
    })
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AlertifyService,
    AuthGuard,
    PreventUnsavedChangesForCollaborator,
    PreventUnsavedChangesForSheets,
    UserService,
    CollaboratorListResolver,
    CollaboratorDetailResolver,
    ProfileEditResolver,
    MessagesResolver,
    StrategiesResolver,
    StrategyListResolver,
    StrategyDetailResolver,
    BehavioralSkillDetailResolver,
    EvaluationHrDetailResolver,
    AdminService,
    HrService,
    LayoutService,
    SheetsResolver,
    SheetDetailResolver,
    ReportSheetsResolver
  ],
  entryComponents: [
    RolesModalComponent,
    AxisModalComponent,
    StrategyEditModalComponent,
    BehavioralSkillEditModalComponent,
    EvaluationHrEditModalComponent,
    CollaboratorSearchComponent,
    GoalEditModalComponent,
    PromptModalComponent,
    GoalEvaluationModalComponent,
    CascadeMyGoalsModalComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
