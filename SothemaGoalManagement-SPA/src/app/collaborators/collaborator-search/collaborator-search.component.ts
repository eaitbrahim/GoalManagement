import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap';

import { User } from '../../_models/user';
import { AdminService } from '../../_services/admin.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Evaluator } from '../../_models/evaluator';
import { UserStatus } from '../../_models/userStatus';

@Component({
  selector: 'app-collaborator-search',
  templateUrl: './collaborator-search.component.html',
  styleUrls: ['./collaborator-search.component.css']
})
export class CollaboratorSearchComponent implements OnInit {
  @Input() actionLabel: string;
  @Input() userStatusList: UserStatus[] = [];
  @Output() actionEvent = new EventEmitter<User[]>();
  users: any;
  searchTerm: { userToSearch: string, userStatusId: number };
  public loading = false;
  selectedAll: any;
  selectedUsers: User[] = [];

  constructor(private adminService: AdminService, public bsModalRef: BsModalRef, private alertify: AlertifyService) { }

  ngOnInit() {
    this.searchTerm = { userToSearch: '', userStatusId: 0 };
  }

  selectAll() {
    this.selectedAll = !this.selectedAll;
    for (let i = 0; i < this.users.length; i++) {
      this.users[i].selected = this.selectedAll;
    }
  }

  checkIfAllSelected() {
    let totalSelected = 0;
    for (let i = 0; i < this.users.length; i++) {
      if (this.users[i].selected) {
        totalSelected++;
      }
    }
    this.selectedAll = totalSelected === this.users.length;

    return true;
  }

  resetSearch() {
    this.searchTerm = { userToSearch: '', userStatusId: 0 };
    this.users = [];
  }

  searchUsers() {
    this.loading = true;
    this.adminService.searchUsers(this.searchTerm).subscribe(users => {
      this.users = users;
      this.loading = false;
      if (this.users.length === 0) {
        this.alertify.error(`Aucun utilisateur trouver!`);
      }
    },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  executeAction() {
    this.selectedUsers = [];
    let selectedUser = {} as User;
    for (let i = 0; i < this.users.length; i++) {
      if (this.users[i].selected) {
        selectedUser = Object.assign({}, this.users[i]);
        this.selectedUsers.push(selectedUser);
      }
    }
    this.actionEvent.emit(this.selectedUsers);
    this.bsModalRef.hide();
  }

  disableAction() {
    if (this.users === undefined) {
      return true;
    }
    for (let i = 0; i < this.users.length; i++) {
      if (this.users[i].selected) {
        return false;
      }
    }
    return true;
  }
}
