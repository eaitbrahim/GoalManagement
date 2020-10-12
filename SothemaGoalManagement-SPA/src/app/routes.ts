import { Component } from '@angular/core';
import { Routes } from '@angular/router';

import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { CollaboratorEditComponent } from './collaborators/collaborator-edit/collaborator-edit.component';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HrPanelComponent } from './hr/hr-panel/hr-panel.component';
import { StrategiesComponent } from './strategies/strategies.component';
import { StrategiesResolver } from './_resolvers/strategies.resolver';
import { CollaboratorListResolver } from './_resolvers/collaborator-list.resolver';
import { CollaboratorDetailResolver } from './_resolvers/collaborator-detail.resolver';
import { CollaboratorDetailComponent } from './collaborators/collaborator-detail/collaborator-detail.component';
import { StrategyListResolver } from './_resolvers/strategy-list.resolver';
import { StrategyDetailComponent } from './hr/strategies/strategy-detail/strategy-detail.component';
import { StrategyDetailResolver } from './_resolvers/strategy-detail.resolver';
import { ResetPasswordComponent } from './collaborators/reset-password/reset-password.component';
import { ForgetPasswordComponent } from './collaborators/forget-password/forget-password.component';
import { ProfileEditComponent } from './collaborators/profile-edit/profile-edit.component';
import { ProfileEditResolver } from './_resolvers/profile-edit.resolver';
import { BehavioralSkillDetailComponent } from './hr/behavioral-skills/behavioral-skill-detail/behavioral-skill-detail.component';
import { BehavioralSkillDetailResolver } from './_resolvers/behavioral-skill-detail.resolver';
import { EvaluationHrDetailComponent } from './hr/evaluations/evaluation-hr-detail/evaluation-hr-detail.component';
import { EvaluationHrDetailResolver } from './_resolvers/evaluation-hr-detail.resolver';
import { SheetsPanelComponent } from './sheets/sheets-panel/sheets-panel.component';
import { SheetsResolver } from './_resolvers/sheets.resolver';
import { SheetDetailComponent } from './sheets/sheet-detail/sheet-detail.component';
import { SheetDetailResolver } from './_resolvers/sheet-detail.resolver';
import { MyCollaboratorGoalsComponent } from './sheets/my-collaborator-goals/my-collaborator-goals.component';
import { MyCollaboratorGoalsResolver } from './_resolvers/my-collaborator-goals.resolver';
import { CollaboratorMessagesComponent } from './collaborators/collaborator-messages/collaborator-messages.component';
import { PreventUnsavedChangesForCollaborator } from './_guards/prevent-unsave-changes-guards-for-collaborator';
import { PreventUnsavedChangesForSheets } from './_guards/prevent-unsave-changes-guards-for-sheets';
import { ReportsPanelComponent } from './reports/reports-panel/reports-panel.component';
import { ReportSheetsResolver } from './_resolvers/report-sheets.resolver';

export const appRoutes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'resetPassword',
    component: ResetPasswordComponent
  },
  {
    path: 'requestResetPassword',
    component: ForgetPasswordComponent
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'profile/edit',
        component: ProfileEditComponent,
        resolve: { user: ProfileEditResolver },
        canDeactivate: [PreventUnsavedChangesForCollaborator]
      },
      {
        path: 'sheets',
        component: SheetsPanelComponent,
        resolve: { resolvedData: SheetsResolver },
        canDeactivate: [PreventUnsavedChangesForSheets]
      }, {
        path: 'sheets/:id',
        component: SheetDetailComponent,
        resolve: { resolvedData: SheetDetailResolver }
      },
      {
        path: 'strategies',
        component: StrategiesComponent,
        resolve: { strategies: StrategiesResolver }
      }, {
        path: 'strategies/:id',
        component: StrategyDetailComponent,
        resolve: { strategy: StrategyDetailResolver }
      }, {
        path: 'behavioralSkills/:id',
        component: BehavioralSkillDetailComponent,
        resolve: { behavioralSkill: BehavioralSkillDetailResolver }
      },
      {
        path: 'evaluationFiles/:id',
        component: EvaluationHrDetailComponent,
        resolve: { evaluationFile: EvaluationHrDetailResolver }
      },
      {
        path: 'messages',
        component: MessagesComponent,
        resolve: { messages: MessagesResolver }
      },
      {
        path: 'hr',
        component: HrPanelComponent,
        resolve: { strategies: StrategyListResolver },
        data: { roles: ['HR'] }
      },
      {
        path: 'reports',
        component: ReportsPanelComponent,
        resolve: { resolvedData: ReportSheetsResolver },
        data: { roles: ['HR'] }
      },
      {
        path: 'admin',
        component: AdminPanelComponent,
        resolve: { resolvedData: CollaboratorListResolver },
        data: { roles: ['Admin', 'HR'] }
      },
      {
        path: 'admin/collaborators/:id',
        component: CollaboratorDetailComponent,
        resolve: { user: CollaboratorDetailResolver }
      },
      {
        path: 'admin/collaborators/edit/:id',
        component: CollaboratorEditComponent,
        resolve: { user: CollaboratorDetailResolver },
        canDeactivate: [PreventUnsavedChangesForCollaborator]
      }
    ]
  },
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full'
  }
];
