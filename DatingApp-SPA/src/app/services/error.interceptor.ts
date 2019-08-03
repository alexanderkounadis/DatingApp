import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor{
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>>{
        return next.handle(req).pipe(
            catchError(error => {
                if(error instanceof HttpErrorResponse){

                    // 401
                    if(error.status === 401) {
                        return throwError(error.statusText);
                    }
                    const appError = error.headers.get('Application-Error');
                    if(appError) {
                        console.error(appError);
                        return throwError(appError);
                    }
                    const serverError = error.error.errors;
                    let modalStateErrors = '';
                    if(serverError && typeof serverError === 'object'){
                        for(const key in serverError){
                            if(serverError[key]){
                                modalStateErrors += serverError[key] + '\n';
                            }
                        }
                    }
                    return throwError (modalStateErrors || serverError || 'Server error');
                }
            })
        );
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi:true
}