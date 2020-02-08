import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import {User} from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './../_services/auth.service';

@Injectable()
export class MemberEditRessolver implements Resolve<User> {

    constructor(private userService: UserService, private router: Router, private alertify: AlertifyService, private authService: AuthService) {}
resolve(route: ActivatedRouteSnapshot): Observable<User> {
    console.log('here');
    console.log(this.authService.decodedToken.nameid);
    return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
        catchError(error => {
            console.log(error);
            this.alertify.error('Problem retrieving your data');
            this.router.navigate(['/members']);
            return of(null);
        })
    );
}
}

// resolver are used to fetch data before the route is activated. So here we get the data based on id. Pass
// that data in the route defined
// And then the init function of that routes component we can get this data here using route.data
