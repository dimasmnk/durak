import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TgService } from '../services/tg.service';
import { from, lastValueFrom, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  if(req.url.endsWith('/auth')) {
    return next(req);
  }

  return from(authService.getToken()).pipe(
    switchMap(token => {
      if (token) {
        const cloned = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });
        return next(cloned);
      } else {
        return next(req);
      }
    })
  );
};
