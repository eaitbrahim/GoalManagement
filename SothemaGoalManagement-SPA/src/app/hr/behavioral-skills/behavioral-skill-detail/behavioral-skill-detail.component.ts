import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import {
  NgxGalleryOptions,
  NgxGalleryImage,
  NgxGalleryAnimation
} from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';

import { BehavioralSkill } from '../../../_models/behavioralSkill';
import { HrService } from '../../../_services/hr.service';
import { AlertifyService } from '../../../_services/alertify.service';
import { AuthService } from '../../../_services/auth.service';

@Component({
  selector: 'app-behavioral-skill-detail',
  templateUrl: './behavioral-skill-detail.component.html',
  styleUrls: ['./behavioral-skill-detail.component.css']
})
export class BehavioralSkillDetailComponent implements OnInit {
  behavioralSkill: BehavioralSkill;
  loading: boolean = false;

  constructor(private hrService: HrService, private authService: AuthService, private alertify: AlertifyService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.behavioralSkill = data['behavioralSkill'];
    });
  }

  clone() {
    this.loading = true;
    this.hrService
      .cloneBehavioralSkill(this.authService.decodedToken.nameid, this.behavioralSkill.id)
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success('La compétence a été clonée avec succès');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  delete() {
    this.alertify.confirm(
      'Etes-vous sur de vouloir supprimer cette compétence?',
      () => {
        this.loading = true;
        this.hrService.deleteBehavioralSkill(this.behavioralSkill.id)
          .subscribe(
            () => {
              this.loading = false;
              this.alertify.success('La compétence a été supprimée');
            },
            error => {
              this.loading = false;
              this.alertify.error('Impossible de supprimer la compétence: ' + error);
            },
            () => {
              this.router.navigate(['/hr'], { queryParams: { tab: 1 } });
            }
          );
      }
    );
  }
}
