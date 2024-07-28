import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './App.css'
import Layout from './layout'

import LoginPage from './pages/login/page'


const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />
  }
])

function App() {

  return (
    <Layout>
      <RouterProvider router={router} />
    </Layout>
  )
}

export default App
