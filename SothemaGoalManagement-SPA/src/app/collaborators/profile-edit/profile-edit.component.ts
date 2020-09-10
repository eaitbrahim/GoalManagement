import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';

import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { User } from '../../_models/user';

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.css']
})
export class ProfileEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
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
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(
      photoUrl => (this.photoUrl = photoUrl)
    );
  }

  updateUser() {
    this.loading = true;
    this.userService
      .updateProfile(this.authService.decodedToken.nameid, this.user)
      .subscribe(
        next => {
          this.loading = false;
          this.alertify.success('Mise à jour du profil réussie');
          this.editForm.reset(this.user);
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }

  cancel() {
    this.router.navigate(['/home']);
  }
}
