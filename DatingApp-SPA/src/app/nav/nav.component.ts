import { AuthService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  userName: string = '';

  constructor(private authService: AuthService,
              private alertifyService: AlertifyService,
              private router: Router) { }

  ngOnInit() {
  }

  login() {
    console.log(!this.authService);
    this.authService.login(this.model).subscribe(next => {
      this.alertifyService.success('Logged in successfully');
    }, error => {
      this.alertifyService.error(error);
    }, ()=>{
      this.router.navigate(['/members']);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertifyService.message('logged out!');
    this.router.navigate(['/home']);
  }
}
