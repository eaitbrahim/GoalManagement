import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { faEdit } from '@fortawesome/free-solid-svg-icons';

import { User } from '../../_models/user';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-roles-management',
  templateUrl: './user-roles-management.component.html',
  styleUrls: ['./user-roles-management.component.css']
})
export class UserRolesManagementComponent implements OnInit {
  @Input() users: User[];
  @Output() loadUsersWithRolesEvent = new EventEmitter<any>();
  @Output() editUserRoleEvent = new EventEmitter<any>();
  bsModalRef: BsModalRef;
  faEdit = faEdit;

  constructor(private modalService: BsModalService) { }

  ngOnInit() { }

  handleLoadUsers(event: any) {
    this.loadUsersWithRolesEvent.emit(event);
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, { initialState, ignoreBackdropClick: true });
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roleNames: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate) {
        let updateParams = { user, rolesToUpdate }
        this.editUserRoleEvent.emit(updateParams);
      }
    });
  }

  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'HR', value: 'HR', },
      { name: 'Collaborator', value: 'Collaborator' },
      { name: 'HRD', value: 'HRD' }
    ];

    for (let i = 0; i < availableRoles.length; i++) {
      let isMatch = false;
      for (let j = 0; j < userRoles.length; j++) {
        if (availableRoles[i].name === userRoles[j]) {
          isMatch = true;
          availableRoles[i].checked = true;
          roles.push(availableRoles[i]);
          break;
        }
      }
      if (!isMatch) {
        availableRoles[i].checked = false;
        roles.push(availableRoles[i]);
      }
    }
    return roles;
  }

}
