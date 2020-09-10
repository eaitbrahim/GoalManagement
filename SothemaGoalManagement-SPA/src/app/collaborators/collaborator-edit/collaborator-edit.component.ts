import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';

import { AlertifyService } from './../../_services/alertify.service';
import { AdminService } from '../../_services/admin.service';
import { AuthService } from '../../_services/auth.service';
import { User } from '../../_models/user';
import { Department } from '../../_models/department';
import { UserStatus } from '../../_models/userStatus';

@Component({
  selector: 'app-collaborator-edit',
  templateUrl: './collaborator-edit.component.html',
  styleUrls: ['./collaborator-edit.component.css']
})
export class CollaboratorEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  bsConfig: Partial<BsDatepickerConfig>;
  bsValue = new Date();
  departmentList: Department[];
  userStatusList: UserStatus[];
  public loading = false;
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private adminService: AdminService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.departmentList = JSON.parse(localStorage.getItem('departmentList'));
    this.userStatusList = JSON.parse(localStorage.getItem('userStatusList'));
    this.bsConfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'YYYY-MM-DD'
    };

    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.bsValue = this.user.recruitmentDate;
  }

  updateUser() {
    this.loading = true;
    const updatedUser = {
      departmentId: this.user.department.id,
      userStatusId: this.user.userStatus.id,
      email: this.user.email,
      employeeNumber: this.user.employeeNumber,
      firstName: this.user.firstName,
      id: this.user.id,
      lastName: this.user.lastName,
      recruitmentDate: this.user.recruitmentDate,
      title: this.user.title
    };
    this.adminService
      .updateUser(updatedUser)
      .subscribe(
        next => {
          this.loading = false;
          this.alertify.success('Mise à jour du profil réussie');
          this.editForm.reset(updatedUser);
        },
        error => {
          this.alertify.error(error);
          this.loading = false;
        }
      );
  }

}
