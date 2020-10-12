import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { faUser, faKey, faSignOutAlt, faBell } from '@fortawesome/free-solid-svg-icons';

import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from './../_services/user.service';
import { Message } from './../_models/message';
import { PaginatedResult } from './../_models/pagination';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  photoUrl: string;
  faSignOutAlt = faSignOutAlt;
  faKey = faKey;
  faUser = faUser;
  faBell = faBell;
  unreadNessages: number;
  public loading = false;


  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private userService: UserService,
  ) {
    router.events.subscribe(e => {
      if (e instanceof NavigationEnd && this.authService.loggedIn()) {
        this.userService.totalUnreadMessages(this.authService.decodedToken.nameid);
      }
    });
  }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(
      photoUrl => (this.photoUrl = photoUrl)
    );

    this.userService.currentUnreadMessages.subscribe(
      unreadMessages => (this.unreadNessages = unreadMessages)
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('Déconnecté');
    this.router.navigate(['/home']);
  }
}
