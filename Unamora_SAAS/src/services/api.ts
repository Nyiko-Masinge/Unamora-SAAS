import axios, { AxiosInstance } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7283/api';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  password: string;
  role: 'Client' | 'Tradesperson';
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
  };
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: API_BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add request interceptor to include token
    this.api.interceptors.request.use((config) => {
      const token = localStorage.getItem('authToken');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Add response interceptor for error handling
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('authToken');
          localStorage.removeItem('refreshToken');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Auth endpoints
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await this.api.post<AuthResponse>('/auth/login', credentials);
    if (response.data.token) {
      localStorage.setItem('authToken', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
    }
    return response.data;
  }

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await this.api.post<AuthResponse>('/auth/register', data);
    if (response.data.token) {
      localStorage.setItem('authToken', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
    }
    return response.data;
  }

  async logout(): Promise<void> {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
  }

  // Profile endpoints
  async getClientProfile(userId: string) {
    return this.api.get(`/clientprofile/${userId}`);
  }

  async updateClientProfile(userId: string, data: any) {
    return this.api.put(`/clientprofile/${userId}`, data);
  }

  async getTradespersonProfile(userId: string) {
    return this.api.get(`/tradespersonprofile/${userId}`);
  }

  async updateTradespersonProfile(userId: string, data: any) {
    return this.api.put(`/tradespersonprofile/${userId}`, data);
  }

  // Search endpoints
  async searchTradespersons(filters: any) {
    return this.api.get('/search', { params: filters });
  }

  // Booking endpoints
  async createJobRequest(data: any) {
    return this.api.post('/booking/job-request', data);
  }

  async getBookings(userId: string) {
    return this.api.get(`/booking/user/${userId}`);
  }

  async getQuotes(userId: string) {
    return this.api.get(`/booking/quotes/${userId}`);
  }

  async createQuote(data: any) {
    return this.api.post('/booking/quote', data);
  }

  // Chat endpoints
  async getChatConversations(userId: string) {
    return this.api.get(`/chat/conversations/${userId}`);
  }

  async getMessages(conversationId: string) {
    return this.api.get(`/chat/messages/${conversationId}`);
  }

  async sendMessage(data: any) {
    return this.api.post('/chat/message', data);
  }

  // Payment endpoints
  async getPayments(userId: string) {
    return this.api.get(`/payment/user/${userId}`);
  }

  async createPayment(data: any) {
    return this.api.post('/payment', data);
  }

  // Review endpoints
  async getReviews(userId: string) {
    return this.api.get(`/review/user/${userId}`);
  }

  async createReview(data: any) {
    return this.api.post('/review', data);
  }

  // Admin endpoints
  async getAdminDashboard() {
    return this.api.get('/admin/dashboard');
  }

  async getVerificationQueue() {
    return this.api.get('/admin/verification-queue');
  }

  async getDisputes() {
    return this.api.get('/admin/disputes');
  }
}

export const api = new ApiService();
