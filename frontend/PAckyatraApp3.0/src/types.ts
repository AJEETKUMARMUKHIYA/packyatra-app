export interface User {
  username?: string;
  firstName?: string;
  name?: string;
  token?: string;
  accessToken?: string;
}

export interface Job {
  id: number;
  name: string;
  location: string;
  type: string;
  date: string;
  time: string;
  status: 'pending' | 'transit' | 'completed' | string;
}

export interface Comment {
  id: number;
  user: string;
  text: string;
  time: string;
}
