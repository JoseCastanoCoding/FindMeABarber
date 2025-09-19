import { UserResponseDto } from './UserResponseDto';

export interface AuthResponseDto {
  token: string;       // JWT token
  user: UserResponseDto;
}