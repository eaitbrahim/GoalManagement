import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  NgxGalleryOptions,
  NgxGalleryImage,
  NgxGalleryAnimation
} from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';
import { faTrash } from '@fortawesome/free-solid-svg-icons';

import { Strategy } from '../../../_models/strategy';
import { Axis } from './../../../_models/axis';
import { HrService } from '../../../_services/hr.service';
import { AlertifyService } from '../../../_services/alertify.service';
import { AuthService } from '../../../_services/auth.service';
import { AxisPole } from '../../../_models/axisPole';

@Component({
  selector: 'app-strategy-detail',
  templateUrl: './strategy-detail.component.html',
  styleUrls: ['./strategy-detail.component.css']
})
export class StrategyDetailComponent implements OnInit {
  strategy: Strategy;
  axisList: Axis[];
  public loading: boolean = false;
  isReadOnly: boolean;
  faTrash = faTrash;
  tallyWeights: any = {};
  messages: any[] = [];
  axisPoles: AxisPole[] = [];
  updatedAxisId: number;

  constructor(private hrService: HrService, private authService: AuthService, private alertify: AlertifyService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.strategy = data['strategy'];
      this.isReadOnly = this.strategy.sealed;
    });

    this.loadAxisList(this.strategy.id);
  }

  loadAxisList(strategyId) {
    this.loading = true;
    this.hrService.getAxisList(strategyId).subscribe(
      axises => {
        this.loading = false;
        this.axisList = axises;
        this.tallyWeightsPerPole();
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      }
    );
  }

  handleAddAxis(strategyId: number, newAxis: Axis) {
    newAxis.strategyId = strategyId;
    this.loading = true;
    this.hrService
      .addAxis(newAxis)
      .subscribe(
        (axis: Axis) => {
          this.loading = false;
          this.loadAxisList(strategyId);
          // this.axisList.unshift(axis);
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }

  clone() {
    this.loading = true;
    this.hrService
      .cloneStrategy(this.authService.decodedToken.nameid, this.strategy.id)
      .subscribe(
        () => {
          this.loading = false;
          this.alertify.success('La stratégie a été clonée avec succès');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        },
        () => {
          this.router.navigate(['/hr']);
        }
      );
  }

  delete() {
    this.alertify.confirm('Supprimer',
      'Êtes-vous sûr de vouloir supprimer cette stratégie?',
      () => {
        this.loading = true;
        this.hrService.deleteStrategy(this.strategy.id)
          .subscribe(
            () => {
              this.loading = false;
              this.alertify.success('La stratégie a été supprimée');
            },
            error => {
              this.loading = false;
              this.alertify.error('Impossible de supprimer la stratégie: ' + error);
            },
            () => {
              this.router.navigate(['/hr']);
            }
          );
      }
    );
  }

  tallyWeightsPerPole() {
    this.axisPoles = [];
    this.axisList.forEach(axis => this.axisPoles = [...this.axisPoles, ...axis.axisPoles]);
    this.tallyWeights = this.axisPoles.reduce((tally, pole) => {
      if (tally[pole.poleName]) tally[pole.poleName] = parseInt(tally[pole.poleName]) + pole.weight;
      else tally[pole.poleName] = pole.weight;
      return tally;
    }, {});

    if (this.messages.length > 0) this.messages = [];
    for (let key in this.tallyWeights) {
      this.messages.push({ poleName: key, totalWeight: this.tallyWeights[key] });
    }
  }

  handleUpdateAxisPole(axisPole: AxisPole) {
    this.loading = true;
    this.hrService
      .updateAxisPoleWeigth(axisPole.axisId, axisPole.poleId, axisPole.weight)
      .subscribe(
        next => {
          this.loading = false;
          this.loadAxisList(this.strategy.id);
          this.updatedAxisId = axisPole.axisId;
          this.alertify.success('Mise à jour du pondération est réussie');
        },
        error => {
          this.loading = false;
          this.alertify.error(error);
        }
      );
  }
}
